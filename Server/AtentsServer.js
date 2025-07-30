mongo = require('./mongo');
const loadData = require('./DataLoader');
const { UpdateDataFromCollection } = require('./mongo');
const fs = require('fs');
const path = require('path');


jwt = require('jsonwebtoken');

const RECRUIT_JEM = 100;    // Recruit에 필요한 젬의 양

const levelData = loadData.loadLevelData();

//#region jwt 토큰 인증 미들웨어
async function AuthenticateToken(req, res, next) {
  const authHeader = req.headers['authorization'];
  const token = authHeader && authHeader.split(' ')[1]; // Bearer TOKEN

  if (!token) return res.sendStatus(401);

  let user_info = await mongo.FindDataFromCollection("Users", req.query.userId);

  jwt.verify(token, user_info._secrete, (err, user) => {
    if (err) return res.sendStatus(403);
    req.user = user;
    next();
  });
}
//#endregion


//#region GET 메소드
async function EndStage(req, res){
    setTimeout(() => { }, 3000);

    let user_info = await mongo.FindDataFromCollection("Users", req.query.userId);
    let stage_info = await mongo.FindDataFromCollection("Stages", req.query.stage);
    let rewards = []
    for(let i = 0; i < stage_info._info.rewards.first.length; i++){
        const item = {
            id: stage_info._info.rewards.first[i].name,
            count: stage_info._info.rewards.first[i].value
        };
        rewards.push(item);
    }
    for(let i = 0; i < stage_info._info.rewards.repeated.length; i++){
        let min = stage_info._info.rewards.repeated[i].min;
        let max = stage_info._info.rewards.repeated[i].max;
        const item = {
            id: stage_info._info.rewards.repeated[i].name,
            count: Math.floor(Math.random() * (max - min + 1)) + min
        }
        rewards.push(item);
    }
    rewards.forEach(reward => {
        if(reward.name == "Gold"){
            mongo.UpdateDataFromCollection("Users", req.query.userId, {
                "_info.currency.gold": reward.count + user_info._info.currency.gold
            });
        }
        else if(reward.name == "Jem"){
            mongo.UpdateDataFromCollection("Users", req.query.userId, {
                "_info.currency.jem": reward.count + user_info._info.currency.jem
            });
        }
    })
    response = {
        rewards: rewards,
    }
    res.json(response);
}

async function UserInfo(req, res){
    let user_info = await mongo.FindDataFromCollection("Users", req.query.userId);
    response = {
        name: user_info._info.name,
        level: user_info._info.lv,
        exp: user_info._info.exp/levelData[user_info._info.lv],
        profile: user_info._info.profile,
        currency: user_info._info.currency,
        homeCharacter: user_info._info.homeCharacter,
        owningCharacters: user_info._info.owningCharacters,
        enableStages: user_info._info.enableStages,
        characterLineup: user_info._info.characterLineup,
        inventory: user_info._info.inventory
    }
    res.json(response);
}

async function CharacterInfo(req, res){
    let character_info = await mongo.FindDataFromCollection("Characters", req.query.character);
    let skillTable = await mongo.FindDataFromCollection("DataSheet", "SkillTable");

    response = {
        name: character_info._id,
        status: character_info._info.status,
        targetingMethod: character_info._info.status.targetingMethod,
        skillSet: character_info._info.skillSet
    }
    res.json(response);
}

async function GlobalInfo(req, res){
    let global_data = await mongo.FindDataFromCollection("DataSheet", "GlobalData");

    response = {
        events: global_data._data.events
    }
    res.json(response);
}
//#endregion

//#region POST 메소드
async function StartStage(req, res){
    let user_info = await mongo.FindDataFromCollection("Users", req.body.userId);
    enable_stages = [];
    user_info._info.enableStages.forEach(stage => enable_stages.push(stage.name));

    //유저가 진입할 수 있는 스테이지 일 겨우
    if(enable_stages.includes(req.body.stage)){
        let stage_info = await mongo.FindDataFromCollection("Stages", req.body.stage);
        characters = new Set(user_info._info.characterLineup);
        for(let i = 0; i < stage_info._info.substages.length; i++){
            stage_info._info.substages[i].enemies.forEach(character => characters.add(character));
        }
        let characterDic = [];
        for(const chr of characters){
            if(chr == "None") continue;
            let character = await mongo.FindDataFromCollection("Characters", chr);
            characterDic.push({
                name: chr,
                status: character._info.status,
                targetingMethod: character._info.status.targetingMethod,
                skillSet: character._info.skillSet
            });
        }
        response = {
            name: stage_info._id,
            canEnter: true,
            characters: characterDic,
            team: user_info._info.characterLineup,
            substages: stage_info._info.substages
        }
        res.json(response);
    }
    
    //유저가 진입할 수 없는 스테이지 일 경우
    else{
        res.json({ canEnter: false});
    }
}

async function UpdateCharacterLineup(req, res){
    let user = await mongo.FindDataFromCollection("Users", req.body.userId);
    let lineup = user._info.characterLineup;
    lineup[req.body.index] = req.body.character;
    mongo.UpdateDataFromCollection("Users", req.body.userId, {
        "_info.characterLineup": lineup
    });
    let updatedUser = await mongo.FindDataFromCollection("Users", req.body.userId);
    response = {
        characterLineup: updatedUser._info.characterLineup
    }
    res.json(response);
}

async function Recruit(req, res){
    let event_data = await mongo.FindDataFromCollection("Events", req.body.eventId);
    let user_info = await mongo.FindDataFromCollection("Users", req.body.userId);
    if(user_info._info.currency.jem < RECRUIT_JEM){
        res.json({ status: "error", message: "Not enough Jem" });
        return;
    }
    let rand = Math.random();
    let selectedGroup = -1;
    for(let i = 0; i < event_data._data.groups.length; i++){
        if(rand < event_data._data.groups[i].rate){
            selectedGroup = i;
            break;
        }
        else{
            rand -= event_data._data.groups[i].rate;
        }
    }
    let response = {};
    if(selectedGroup >= 0){
        let selectedCharacters = event_data._data.groups[selectedGroup].characters;
        const rand = Math.floor(Math.random() * selectedCharacters.length);
        let selectedCharacter = selectedCharacters[rand];
        let owningCharacters = user_info._info.owningCharacters;
        const characterSet = new Set(owningCharacters);
        characterSet.add(selectedCharacter);
        owningCharacters = Array.from(characterSet);
        await mongo.UpdateDataFromCollection("Users", req.body.userId, {
            "_info.currency.jem": user_info._info.currency.jem - RECRUIT_JEM,
            "_info.owningCharacters": owningCharacters,
        });
        user_info = await mongo.FindDataFromCollection("Users", req.body.userId);
        response = {
            status: "success",
            message: "Recruit Success!",
            recruitedCharacter: selectedCharacter,
            updatedUserInfo: user_info._info
        };
    }
    else{
        response = {
            status: "error",
            message: "Recruit Failed! Please try again later."
        };
    }
    console.log("Recruit Result: ", response);
    res.json(response);
}

async function UseItem(req, res) {
    let item = req.body.itemId;
    switch(item) {
        case "gold_bundle":
            let result = await UseGoldBundle(req.body.userId);
            let updated_user_info = await mongo.FindDataFromCollection("Users", req.body.userId);
            res.json({ 
                status: "success", 
                message: result,
                updatedUserInfo: {
                    currency: updated_user_info._info.currency,
                    inventory: updated_user_info._info.inventory
                }
            });
            break;
        default:
            res.json({ 
                status: "error", 
                message: "Unknown item",
                updatedUserInfo: {
                    currency: user_info._info.currency,
                    inventory: user_info._info.inventory
                }
            });
            break;
    }
}

async function Login(req, res) {
    let user = await mongo.FindDataFromCollection("Users", req.body.id);
    if(user){
        if (user._pw === req.body.password) {
            res.json({ status: "success", message: "Login Success!", token: jwt.sign({ id: user._id }, user._secrete, { expiresIn: '1h' }), id: req.body.id });
        } else {
            res.json({ status: "error", message: "Incorrect ID or password", token: "-", id: "-" });
        }
    }
    else {
        res.json({ status: "error", message: "Incorrect ID or password", token: "-", id: "-" });
    }
}
//#endregion

//#region 아이템 사용시 함수
async function UseGoldBundle(userId){
    let user_info = await mongo.FindDataFromCollection("Users", userId);
    let inventory = user_info._info.inventory;
    for (let i = 0; i < inventory.length; i++) {
        if (inventory[i].id === "gold_bundle") {
            if (inventory[i].count >= 1) {
                if(inventory[i].count > 1) {
                    inventory[i].count -= 1;
                }
                else if(inventory[i].count === 1) {
                    inventory.splice(i, 1);
                }
                break;
            } 
            else{
                return "아이템이 부족합니다.";
            }
        }
    }
    await mongo.UpdateDataFromCollection("Users", userId, {
        "_info.currency.gold": user_info._info.currency.gold + 1000,
        "_info.inventory": inventory
    });
    return "골드를 1000 얻었습니다.";
}
//#endregion

exports.AuthenticateToken = AuthenticateToken;
exports.EndStage = EndStage;
exports.Login = Login;
exports.UserInfo = UserInfo;
exports.CharacterInfo = CharacterInfo;
exports.GlobalInfo = GlobalInfo;
exports.StartStage = StartStage;
exports.Recruit = Recruit;
exports.UpdateCharacterLineup = UpdateCharacterLineup;
exports.UseItem = UseItem;