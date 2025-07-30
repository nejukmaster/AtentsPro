const MongoClient = require("mongodb").MongoClient;

const mongodb_usrname = 'admin';
const mongodb_pw = 'atentspro_backend';

const mongodb_uri = 'mongodb+srv://'+mongodb_usrname+':'+mongodb_pw+'@cluster0.gxughgs.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0';

async function GetDataFromCollection(name){
    const client = new MongoClient(mongodb_uri);
    try {
        const db = client.db("AtentsProDB");
        const todos_collection = db.collection(name);
        return todos = await todos_collection.find().toArray();
    } catch (error) {
        console.error("Error retrieving todos:", error);
        return res.json({ message: "Fail to retrieve data." });
    }
}

async function FindDataFromCollection(name,id){
    const client = new MongoClient(mongodb_uri);
    try {
        const db = client.db("AtentsProDB");
        const todos_collection = db.collection(name);
        return todos = await todos_collection.findOne({_id: id});
    } catch (error) {
        console.error("Error retrieving todos:", error);
        return { message: "Fail to retrieve data." };
    }
}

async function UpdateDataFromCollection(name, id, data){
    const client = new MongoClient(mongodb_uri);
    try {
        const db = client.db("AtentsProDB");
        const todos_collection = db.collection(name);
        const result = await todos_collection.updateOne({_id: id}, {$set: data});
        return result;
    } catch (error) {
        console.error("Error updating data:", error);
        return { message: "Fail to update data." };
    }
}

exports.GetDataFromCollection = GetDataFromCollection;
exports.FindDataFromCollection = FindDataFromCollection;
exports.UpdateDataFromCollection = UpdateDataFromCollection;