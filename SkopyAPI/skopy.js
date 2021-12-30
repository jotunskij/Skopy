// State for simulation
var skopySolveStateOld;
var skopySolveStateNew;

// Misc variables
var minX;
var minY;

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

    minX = toyXMin < treeXMin ? Math.abs(toyXMin) : Math.abs(treeXMin);
    minY = toyYMin < treeYMin ? Math.abs(toyYMin) : Math.abs(treeYMin);
    console.log('minY: ' + minY);
    console.log('minX: ' + minX);
}

// Initialize two.js
var params = {
    fullscreen: false
};
var elem = document.getElementById('canvas');
var two = new Two(params).appendTo(elem);
two.width = 1000;
two.height = 1000;

// API
const apiLoadFile = async (file) => {
    const response = await fetch(`http://localhost:5200/loadFile/${file}`);
    skopySolveStateNew = await response.json();
    console.log(skopySolveStateNew);
}

const apiSolve = async () => {
    console.log(skopySolveStateNew.value);
    const response = await fetch('http://localhost:5200/solve', {
        method: 'POST',
        body: JSON.stringify(skopySolveStateNew.value),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    //skopySolveStateOld = skopySolveStateNew;
    skopySolveStateNew = await response.json();
}

async function loadProblemFile() {
    var filename = document.getElementById('problemFileName').value;
    await apiLoadFile(filename);
    getMinimums();
    clearAndDraw();
}

async function runSolveStep() {
    await apiSolve();
    clearAndDraw();
}

function clearAndDraw() {
    two.clear();

    const coordScaleFactor = 0.05;

    skopySolveStateNew.value.trees.forEach((tree) => {
        var treeCircle = two.makeCircle((tree.coord.x + minX) * coordScaleFactor, (tree.coord.y + minY) * coordScaleFactor, 5);
        treeCircle.fill = '#FF8000';
    });

    skopySolveStateNew.value.toys.forEach((toy) => {
        var toyRect = two.makeRectangle((toy.coord.x + minX) * coordScaleFactor, (toy.coord.y + minY) * coordScaleFactor, 5, 5);
        toyRect.fill = '#000000';
    });

    var currentPos = skopySolveStateNew.value.currentPos;
    var skopyStar = two.makeStar((currentPos.x + minX) * coordScaleFactor, (currentPos.y + minY) * coordScaleFactor, 10, 10, 5);
    skopyStar.fill = '#00FF00';

    two.update();
}

/*
// Two.js has convenient methods to make shapes and insert them into the scene.
var radius = 50;
var x = two.width * 0.5;
var y = two.height * 0.5 - radius * 1.25;
var circle = two.makeCircle(x, y, radius);

y = two.height * 0.5 + radius * 1.25;
var width = 100;
var height = 100;
var rect = two.makeRectangle(x, y, width, height);

// The object returned has many stylable properties:
circle.fill = '#FF8000';
// And accepts all valid CSS color:
circle.stroke = 'orangered';
circle.linewidth = 5;

rect.fill = 'rgb(0, 200, 255)';
rect.opacity = 0.75;
rect.noStroke();

// Don’t forget to tell two to draw everything to the screen
two.update();
*/