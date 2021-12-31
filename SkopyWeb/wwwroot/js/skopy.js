// Simulation state
var skopySolveStates = [];

// Misc variables
var minX;
var minY;
var maxX;
var maxY;
var scaleFactor;
const screenPadding = 100;
const canvasWidth = 500;
const canvasHeight = 500;

function scaleCoord(coord) {
    return {
        'x': (coord.x + minX) * scaleFactor + screenPadding,
        // Invert y (0,0 has y going +down, while cartesian has y going -down)
        'y': (-coord.y + minY) * scaleFactor + screenPadding
    };
}

function currentSolveState() {
    return skopySolveStates[skopySolveStates.length - 1];
}

// Pad the canvas to make sure we have a little space
// for the edge values
function initCanvasSize() {
    two.width = maxX * scaleFactor + screenPadding * 2;
    two.height = maxY * scaleFactor + screenPadding * 2;
}

// Calculate minimum and maximum X/Y values
// From this we can calculate a scale factor to fit the shapes
// to the canvas
function getMinimums() {
    var toyXMin = currentSolveState().value.toys.reduce((acc, toy) => (
        acc < toy.coord.x ? acc : toy.coord.x
    ), Infinity);
    var toyYMin = currentSolveState().value.toys.reduce((acc, toy) => (
        acc < toy.coord.y ? acc : toy.coord.y
    ), Infinity);
    var treeXMin = currentSolveState().value.trees.reduce((acc, tree) => (
        acc < tree.coord.x ? acc : tree.coord.x
    ), Infinity);
    var treeYMin = currentSolveState().value.trees.reduce((acc, tree) => (
        acc < tree.coord.y ? acc : tree.coord.y
    ), Infinity);
    var toyXMax = currentSolveState().value.toys.reduce((acc, toy) => (
        acc > toy.coord.x ? acc : toy.coord.x
    ), -Infinity);
    var toyYMax = currentSolveState().value.toys.reduce((acc, toy) => (
        acc > toy.coord.y ? acc : toy.coord.y
    ), -Infinity);
    var treeXMax = currentSolveState().value.trees.reduce((acc, tree) => (
        acc > tree.coord.x ? acc : tree.coord.x
    ), -Infinity);
    var treeYMax = currentSolveState().value.trees.reduce((acc, tree) => (
        acc > tree.coord.y ? acc : tree.coord.y
    ), -Infinity);

    minX = toyXMin < treeXMin ? Math.abs(toyXMin) : Math.abs(treeXMin);
    minY = toyYMin < treeYMin ? Math.abs(toyYMin) : Math.abs(treeYMin);
    maxX = toyXMax > treeXMax ? Math.abs(toyXMax) : Math.abs(treeXMax);
    maxY = toyYMax > treeYMax ? Math.abs(toyYMax) : Math.abs(treeYMax);
    maxX += minX;
    maxY += minY;
    scaleFactor = maxX > maxY ? canvasWidth / maxX : canvasHeight / maxY;
}

// Initialize two.js
var params = {
    fullscreen: false
};
var elem = document.getElementById('canvas');
var two = new Two(params).appendTo(elem);

// API
let apiLoadFile = async (file) => {
    const response = await fetch(`http://localhost:5200/loadFile/${file}`);
    skopySolveStates = [];
    skopySolveStates.push(await response.json());
    console.log(currentSolveState());
}

let apiSolve = async () => {
    document.getElementById('stepButton').disabled = true;
    const response = await fetch('http://localhost:5200/solve', {
        method: 'POST',
        body: JSON.stringify(currentSolveState().value),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    skopySolveStates.push(await response.json());
    document.getElementById('stepButton').disabled = false;
}

// Load .in file and initialize everything
async function loadProblemFile() {
    var filename = document.getElementById('problemFileName').value;
    await apiLoadFile(filename);
    getMinimums();
    document.getElementById('solveButton').disabled = false;
    document.getElementById('stepButton').disabled = false;
    document.getElementById('undoButton').disabled = true;
    initCanvasSize();
    clearAndDraw();
}

// Solve the entire problem
async function solveUntilEnd() {
    document.getElementById('solveButton').disabled = true;
    while (!currentSolveState().value.solved) {
        await runSolveStep();
    }
}

// Run one step of the solution
async function runSolveStep() {
    await apiSolve();
    clearAndDraw();
    if (skopySolveStates.length > 1) {
        document.getElementById('undoButton').disabled = false;
    }
}

// Back up one step in the solution
async function runUndoStep() {
    skopySolveStates.pop();
    clearAndDraw();
    if (skopySolveStates.length < 2) {
        document.getElementById('undoButton').disabled = true;
    }
    document.getElementById('stepButton').disabled = false;
    document.getElementById('solveButton').disabled = false;
}

// Draw everything on screen
function clearAndDraw() {
    two.clear();

    // Print some helpful data regarding the current state
    var totalToys = currentSolveState().value.toys.length;
    var elem = document.getElementById('debugData');
    elem.textContent = `Max: ${Math.round(currentSolveState().value.longestLength * 100) / 100}, ` +
        `Answer: ${currentSolveState().value.answerFromAnsFile}, ` +
        `Toys: ${currentSolveState().value.currentToyIndex + 1}/${totalToys}, ` +
        `TE: ${currentSolveState().value.traverseList.entries.length}`;

    if (currentSolveState().value.solved) {
        document.getElementById('solveButton').disabled = true;
    }

    // Draw origin
    var origin = scaleCoord({ 'x': 0, 'y': 0 });
    var originCircle = two.makeCircle(origin.x, origin.y, 3);
    originCircle.fill = '#0000FF';

    // Draw trees
    currentSolveState().value.trees.forEach((tree) => {
        var coord = scaleCoord(tree.coord);
        var treeCircle = two.makeCircle(coord.x, coord.y, 5);
        treeCircle.fill = '#FF8000';
    });

    // Draw toys
    currentSolveState().value.toys.forEach((toy) => {
        var coord = scaleCoord(toy.coord);
        var toyRect = two.makeRectangle(coord.x, coord.y, 5, 5);
        toyRect.fill = '#000000';
    });

    // Draw leash
    var traverseLength = currentSolveState().value.traverseList.entries.length;
    for (var i = 0; i < traverseLength - 1; i++) {
        var fromTree = currentSolveState().value.traverseList.entries[i].tree;
        var toTree = currentSolveState().value.traverseList.entries[i + 1].tree;
        fromCoord = scaleCoord(fromTree.coord);
        toCoord = scaleCoord(toTree.coord);
        var line = two.makeLine(fromCoord.x, fromCoord.y, toCoord.x, toCoord.y);
        line.fill = '#444444';
    }

    // Skopys pos
    var currentPos = currentSolveState().value.currentPos;
    var skopyCoord = scaleCoord(currentPos);

    // Draw last leash line
    var lastTree = currentSolveState().value.traverseList.entries[traverseLength - 1].tree;
    var treeCoord = scaleCoord(lastTree.coord);
    var line = two.makeLine(treeCoord.x, treeCoord.y, skopyCoord.x, skopyCoord.y);
    line.fill = '#444444';

    // Draw skopy
    var skopyStar = two.makeStar(skopyCoord.x, skopyCoord.y, 10, 10, 5);
    skopyStar.fill = '#00FF00';

    two.update();
}
