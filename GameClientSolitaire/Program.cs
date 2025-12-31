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

SolitaireGame _solitaireGame = new SolitaireGame(_logger);
_solitaireGame.Play();