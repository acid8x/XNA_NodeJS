var
  express = require('express'),
  app = express(),
  io,
  http;

var id = 0;

http = require('http').createServer(app);
io = require('socket.io')(http, { pingInterval: 500 });
http.listen(2222, function () {
  console.log('socket.io server listening on port', 2222);
});

io.on('connection', function(socket) {
	socket.ident = id++;
	
	socket.emit('id', socket.ident);
	
	console.log("new connection " + socket.ident);
	
  socket.on('position', function(x,y) {
    socket.X = x;
		socket.Y = y;
		socket.broadcast.emit('update', {
			id: socket.ident,
			x: socket.X,
			y: socket.Y
		});
  });
		
	socket.on('disconnect', function() {
		console.log(socket.ident + " disconnected");
  });
});