var app = require("express")();
var http = require("http").Server(app);
var fs = require("fs-extra");
var path = require('path');   
var chokidar = require('chokidar');
var busboy = require('connect-busboy');
var mkdirp = require('mkdirp');

app.use(busboy());
app.use(require("express").static(path.join(__dirname, 'public')));

app.get("/", function(req, res){
	res.send("<h1>Welcome to homeFileSyncTest</h1>");
});

app.post("/create/:relpath", function(req, res){
	console.log(req.protocol + '://' + req.get('host') + req.originalUrl);
	//console.log(req.params.relpath.toString().replace(/\|/g, "\\"));
	var fstream;
        req.pipe(req.busboy);
        req.busboy.on('file', function (fieldname, file, filename) {
            //console.log("Uploading: " + filename);

            fstream = fs.createWriteStream(__dirname + '/files/' + (req.params.relpath.toString().replace(/\|/g, "\\")));
            file.pipe(fstream);
            fstream.on('close', function () {    
                //console.log("Upload Finished of " + filename);              
                res.send('create complete');           //where to go next
            });
        });
});

app.post("/newdir", function(req, res){
	console.log(req.protocol + '://' + req.get('host') + req.originalUrl);
	var fstream;
        req.pipe(req.busboy);
        req.busboy.on('file', function (fieldname, file, filename) {
            //console.log("Uploading: " + filename);

			mkdirp(__dirname + "/files/" + filename.split('.')[0], function(err) { 
				//console.log(__dirname + "/" + filename.split('.')[0]);
			});
            res.send("new dir");
            
        });
});

app.get("/newfolder/:folder", function(req, res){
	console.log(req.protocol + '://' + req.get('host') + req.originalUrl);
	//console.log(req.params.folder);
	//console.log(req.params.folder.toString().replace(/\|/g, "\\"));
	mkdirp(__dirname + "/files/" + req.params.folder.toString().replace(/\|/g, "\\"), function(err) { 
				//console.log(__dirname + "/" + req.params.folder.toString().replace(/\|/g, "\\"));
				res.send("new folder");
	});
});

app.get("/rename/:oldfilename/:newfilename", function(req, res){
	console.log(req.protocol + '://' + req.get('host') + req.originalUrl);
	//console.log(req.params.oldfilename.toString().replace(/\|/g, "\\") + " should be renamed to " + req.params.newfilename.toString().replace(/\|/g, "\\"));
	fs.rename(__dirname + "/files/" + req.params.oldfilename.toString().replace(/\|/g, "\\"), __dirname + "/files/" + req.params.newfilename.toString().replace(/\|/g, "\\"), function(err){
		//console.log("renamed");
	});
	res.send("rename");
});

app.get("/delete/:filename", function(req, res){
	console.log(req.protocol + '://' + req.get('host') + req.originalUrl);
	//console.log(req.params.filename.toString().replace(/\|/g, "\\") + " should be deleted");
	fs.unlink(__dirname + "/files/" + req.params.filename.toString().replace(/\|/g, "\\"), function(err){
		if (!err) {
			//console.log("deleted");
		}
		else {
			//console.log(err);
		}
	});
	res.send("delete");
});

app.get("/deletedir/:foldername", function(req, res){
	console.log(req.protocol + '://' + req.get('host') + req.originalUrl);
	//console.log("delete " + req.params.foldername.toString().replace(/\|/g, "\\"));
	deleteFolderRecursive(__dirname + "/files/" + req.params.foldername.toString().replace(/\|/g, "\\"));
	res.send("delete dir");
});

var deleteFolderRecursive = function(path){
	if (fs.existsSync(path)){
		fs.readdirSync(path).forEach(function(file, index){
			var curPath = path + "/" + file;
			if (fs.lstatSync(curPath).isDirectory()){
				deleteFolderRecursive(curPath);
			}
			else {
				fs.unlinkSync(curPath);
			}
		});
		fs.rmdirSync(path);
	}
};

http.listen(8080, function(){
	console.log("Listening on *:8080");
});