const fs   = require('fs');
const path = require('path');

function loadLevelData() {
  const csvPath = "./LevelDatasheet.csv";
  const text = fs.readFileSync(csvPath, 'utf8');
  const lines = text.trim().split('\n');
  const result = {};

  for(let i = 1; i < lines.length; i++) {
    const cols = lines[i].split(',');
    const level = parseInt(cols[0].trim(),10);
    const maxExp = Number(cols[1].trim());
    result[level] = maxExp;
  }

  return result;
}

exports.loadLevelData = loadLevelData;