// Simulation state
var skopySolveStateNew;

// Misc variables
var minX;
var minY;
var maxX;
var maxY;
var scaleFactor;
const screenPadding = 100;

function scaleCoord(coord) {
    return {
        'x': (coord.x + minX) * scaleFactor + screenPadding,
        // Invert y
        'y': (-coord.y + minY) * scaleFactor + screenPadding
    };
}

function initCanvasSize() {
    two.width = maxX * scaleFactor + screenPadding * 2;
    two.height = maxY * scaleFactor + screenPadding * 2;
}

function getMinimums() {
    var toyXMin = skopySolveStateNew.value.toys.reduce((acc, toy) => (
        acc < toy.coord.x ? acc : toy.coord.x
    ), Infinity);
    var toyYMin = skopySolveStateNew.value.toys.reduce((acc, toy) => (
        acc < toy.coord.y ? acc : toy.coord.y
    ), Infinity);
    var treeXMin = skopySolveStateNew.value.trees.reduce((acc, tree) => (
        acc < tree.coord.x ? acc : tree.coord.x
    ), Infinity);
    var treeYMin = skopySolveStateNew.value.trees.reduce((acc, tree) => (
        acc < tree.coord.y ? acc : tree.coord.y
    ), Infinity);
    var toyXMax = skopySolveStateNew.value.toys.reduce((acc, toy) => (
        acc > toy.coord.x ? acc : toy.coord.x
    ), -Infinity);
    var toyYMax = skopySolveStateNew.value.toys.reduce((acc, toy) => (
        acc > toy.coord.y ? acc : toy.coord.y
    ), -Infinity);
    var treeXMax = skopySolveStateNew.value.trees.reduce((acc, tree) => (
        acc > tree.coord.x ? acc : tree.coord.x
    ), -Infinity);
    var treeYMax = skopySolveStateNew.value.trees.reduce((acc, tree) => (
        acc > tree.coord.y ? acc : tree.coord.y
    ), -Infinity);

    minX = toyXMin < treeXMin ? Math.abs(toyXMin) : Math.abs(treeXMin);
    minY = toyYMin < treeYMin ? Math.abs(toyYMin) : Math.abs(treeYMin);
    maxX = toyXMax > treeXMax ? Math.abs(toyXMax) : Math.abs(treeXMax);
    maxY = toyYMax > treeYMax ? Math.abs(toyYMax) : Math.abs(treeYMax);
    maxX += minX;
    maxY += minY;
    scaleFactor = maxX > maxY ? 500 / maxX : 500 / maxY;
    console.log('minY: ' + minY);
    console.log('minX: ' + minX);
    console.log('maxY: ' + maxY);
    console.log('maxX: ' + maxX);
    console.log('scaleFactor: ' + scaleFactor);
}

// Initialize two.js
var params = {
    fullscreen: false
};
var elem = document.getElementById('canvas');
var two = new Two(params).appendTo(elem);

// API
const apiLoadFile = async (file) => {
    const response = await fetch(`http://localhost:5200/loadFile/${file}`);
    skopySolveStateNew = await response.json();
    console.log(skopySolveStateNew);
}

const apiSolve = async () => {
    document.getElementById('solveButton').disabled = true;
    const response = await fetch('http://localhost:5200/solve', {
        method: 'POST',
        body: JSON.stringify(skopySolveStateNew.value),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    skopySolveStateNew = await response.json();
    document.getElementById('solveButton').disabled = false;
}

async function loadProblemFile() {
    var filename = document.getElementById('problemFileName').value;
    await apiLoadFile(filename);
    getMinimums();
    document.getElementById('solveButton').disabled = false;
    initCanvasSize();
    clearAndDraw();
}

async function runSolveStep() {
    await apiSolve();
    clearAndDraw();
}

function clearAndDraw() {
    two.clear();

    var elem = document.getElementById('longestPath');
    // Round to 2 decimal places
    elem.textContent = `Max: ${Math.round(skopySolveStateNew.value.longestLength * 100) / 100}`;
    var totalToys = skopySolveStateNew.value.toys.length;
    elem = document.getElementById('answer');
    elem.textContent = `Answer: ${skopySolveStateNew.value.answerFromAnsFile}`;
    elem = document.getElementById('toyNr');
    elem.textContent = `Toys: ${skopySolveStateNew.value.currentToyIndex + 1}/${totalToys}`;

    if (skopySolveStateNew.value.solved) {
        document.getElementById('solveButton').disabled = true;
    }

    // Draw origin
    var origin = scaleCoord({ 'x': 0, 'y': 0 });
    var originCircle = two.makeCircle(origin.x, origin.y, 3);
    originCircle.fill = '#0000FF';

    // Draw trees
    skopySolveStateNew.value.trees.forEach((tree) => {
        var coord = scaleCoord(tree.coord);
        var treeCircle = two.makeCircle(coord.x, coord.y, 5);
        treeCircle.fill = '#FF8000';
    });

    // Draw toys
    skopySolveStateNew.value.toys.forEach((toy) => {
        var coord = scaleCoord(toy.coord);
        var toyRect = two.makeRectangle(coord.x, coord.y, 5, 5);
        toyRect.fill = '#000000';
    });

    // Draw leash
    var traverseLength = skopySolveStateNew.value.traverseList.entries.length;
    console.log(traverseLength);
    for (var i = 0; i < traverseLength - 1; i++) {
        var fromTree = skopySolveStateNew.value.traverseList.entries[i].tree;
        var toTree = skopySolveStateNew.value.traverseList.entries[i + 1].tree;
        fromCoord = scaleCoord(fromTree.coord);
        toCoord = scaleCoord(toTree.coord);
        var line = two.makeLine(fromCoord.x, fromCoord.y, toCoord.x, toCoord.y);
        line.fill = '#444444';
    }

    // Skopys pos
    var currentPos = skopySolveStateNew.value.currentPos;
    var skopyCoord = scaleCoord(currentPos);

    // Draw last leash line
    var lastTree = skopySolveStateNew.value.traverseList.entries[traverseLength - 1].tree;
    var treeCoord = scaleCoord(lastTree.coord);
    var line = two.makeLine(treeCoord.x, treeCoord.y, skopyCoord.x, skopyCoord.y);
    line.fill = '#444444';

    // Draw skopy
    var skopyStar = two.makeStar(skopyCoord.x, skopyCoord.y, 10, 10, 5);
    skopyStar.fill = '#00FF00';

    two.update();
}
