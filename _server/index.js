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
	
  socket.on('position', function(h,n,x,y,r,g,b) {
		socket.broadcast.emit('update', {
			hp: h,
			name: n,
			id: socket.ident,
			x: x,
			y: y,
			r: r,
			g: g,
			b: b
		});
  });
	
	socket.on('disconnect', function() {
		console.log(socket.ident + " disconnected");
  });
});