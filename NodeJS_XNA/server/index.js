var
  express = require('express'),
  app = express(),
  io,
  http;

var id = 0;

http = require('http').createServer(app);
io = require('socket.io')(http, { pingInterval: 500 });
http.listen(80, function () {
  console.log('socket.io server listening on port', 80);
});

io.on('connection', function(socket) {
	socket.ident = id++;
	
	socket.emit('id', socket.ident);
	
	console.log(socket.ident + " connected");
	
  socket.on('update', function(data) {
		socket.broadcast.emit('update', data);
	});
	
	socket.on('disconnect', function() {
		console.log(socket.ident + " disconnected");
  });
});