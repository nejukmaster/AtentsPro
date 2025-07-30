const { FindDataFromCollection } = require("./mongo");
const mongoDB = require("./mongo");
const atents_server = require("./AtentsServer");
const express = require("express");

const app = express();

app.use(express.json());

app.post("/atents/login", atents_server.Login);
app.get("/atents/character_info", atents_server.CharacterInfo);
app.get("/atents/global_info", atents_server.GlobalInfo);

app.get("/atents/end_stage", atents_server.AuthenticateToken, atents_server.EndStage);
app.get("/atents/user_info", atents_server.AuthenticateToken, atents_server.UserInfo);
app.post("/atents/start_stage", atents_server.AuthenticateToken, atents_server.StartStage);
app.post("/atents/recruit", atents_server.AuthenticateToken, atents_server.Recruit);
app.post("/atents/update_character_lineup", atents_server.AuthenticateToken, atents_server.UpdateCharacterLineup);
app.post("/atents/use_item", atents_server.AuthenticateToken, atents_server.UseItem);

app.listen(5000, () => console.log("Server is running at 5000 ✨"));