using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JazzApps.Utils
{
    public class Logger
    {
        private const string LOGGER_SYMBOL = "@";
        private static readonly Color LoggerSymbolColor = Color.black;
        private static readonly Color MessageColor = Color.white;
        private static readonly Dictionary<Color, string> colors = new()
        {
            {Color.red, "red"},
            {Color.yellow, "yellow"},
            {Color.green, "blue"},
            {Color.cyan, "cyan"},
            {Color.blue, "blue"},
            {Color.magenta, "magenta"},
            {Color.black, "black"},
            {Color.grey, "grey"},
            {Color.white, "white"},
        };

        private static KeyValuePair<Color, string> GetRandomColorEntry()
        {
            int randomIndex = Random.Range(0, colors.Count);
            var randomEntry = colors.ElementAt(randomIndex);
            return randomEntry;
        }

        private static Dictionary<Object, Color> _senders;
        private static Dictionary<Object, Color> senders
        {
            get
            {
                if (_senders == null)
                    _senders = new();
                return _senders;
            }
        }

        public void ConfigureLog(Object sender, Color newColor)
        {
            if (!colors.ContainsKey(newColor))
                return;
            senders.TryAdd(sender, newColor);
            senders[sender] = newColor;
        }
        
        public static void Log(Object sender, string message, Color prefixColor = default)
        {
            if (prefixColor == default)
                prefixColor = GetRandomColorEntry().Key;
            senders.TryAdd(sender, prefixColor);
            DoLog(senders[sender], sender.name, message, sender);
        }
        private static void DoLog(Color prefixColor, string prefix, string message, Object sender)
        {
            Debug.Log($"<color={colors[LoggerSymbolColor]}>{LOGGER_SYMBOL}</color> <color={colors[prefixColor]}>{prefix}:</color> <color={colors[MessageColor]}>{message}</color>", sender);
        }
        public static void Log(string message)
        {
            Debug.Log($"<color={colors[LoggerSymbolColor]}>{LOGGER_SYMBOL}</color> <color={colors[Color.white]}>:</color> <color={colors[MessageColor]}>{message}</color>");
        }
    }
}
