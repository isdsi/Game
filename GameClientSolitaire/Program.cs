using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using GameClientSolitaire;

ILoggerFactory _loggerFactory;
ILogger _logger;

_loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
_logger = _loggerFactory.CreateLogger("GameClient");

Solitaire _solitaire = new Solitaire(_logger, 777);
Game _game = new Game(_solitaire);
_game.Play();