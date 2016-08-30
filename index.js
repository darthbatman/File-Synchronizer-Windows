var express = require("express");
var app = express();
var http = require("http").Server(app);
var static = require('node-static');
var fs = require('fs');
var bodyParser = require('body-parser');
var methodOverride = require('method-override');
var path = require('path');
var multer = require('multer');
var busboy = require('connect-busboy');

app.use(busboy());
app.use('/files', express.static(__dirname + '/files'));
app.use(express.static(__dirname + '/public'));
app.use(bodyParser.urlencoded({
	extended: true
}));
app.use(bodyParser.json());

var lastPath = "/files/";

var uploading = multer({
  dest: __dirname + '../public/uploads/',
});

app.get('/*', function (req, res) {
  if (req.url.indexOf(".") != -1){
    //download file
    // var fullUrl = req.protocol + '://' + req.get('host') + req.originalUrl;
    // console.log(fullUrl + req.url);
    // res.redirect("http://localhost:8080/" + req.url);
  }
  else {
    console.log(req.url);
    lastPath = req.url;
    var files = fs.readdirSync('.' + req.url);
    var fullUrl = req.protocol + '://' + req.get('host') + req.originalUrl;
    var aURL =  req.protocol + '://' + req.get('host');
    console.log(fullUrl);
    var indexString = "<body style=\"background-color: #bdc3c7\"><p style=\"font-size: 2em; font-family: Tahoma\">File Server</p><br><form action=\"" + aURL + "/upload\" method=\"post\" enctype=\"multipart/form-data\"><input type=\"text\" value=\"" + req.url + "\" readonly/><br><input type=\"file\" name=\"uploadFile\"/><input type=\"submit\"/></form></body>";
    console.log(files);
    var fileString = "";
    for (var i = 1; i < req.url.length - 1; i++){
      fileString += req.url[i];
    }
    files.forEach(function(file){
      indexString += "<p><a href=\"" + fullUrl + file.toString() + "\">" + file.toString() + "</a></p>";
    });
    res.send(indexString);
  }

});

app.post('/upload', function(req, res) {
    var fstream;
    req.pipe(req.busboy);
    req.busboy.on('file', function (fieldname, file, filename) {
        console.log("Uploading: " + filename);
        fstream = fs.createWriteStream(__dirname + lastPath + filename);
        file.pipe(fstream);
        fstream.on('close', function () {
            res.redirect('back');
        });
    });
    req.busboy.on('field', function(fieldname, val, fieldnameTruncated, valTruncated, encoding, mimetype) {
      console.log('Field [' + fieldname + ']: value: ' + inspect(val));
    });
});

http.listen(8080, function(){
   console.log("Listening on *:8080");
});
