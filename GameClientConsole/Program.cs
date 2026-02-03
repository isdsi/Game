using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using GameClientConsole;
using GameClientPoco;

ILoggerFactory _loggerFactory;
ILogger _logger;

_loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
_logger = _loggerFactory.CreateLogger("GameClient");

Game _game = new Game(_logger);
_game.Play();