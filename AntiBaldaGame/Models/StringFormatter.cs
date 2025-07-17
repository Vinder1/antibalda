using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiBaldaGame.Models;

public static class StringFormatter
{
    private const string Alphabet = "йцукенгшщзхъфывапролджэячсмитьбю";

    //TODO Протестировать
    public static char LeaveOneCharacter(string? input)
    {
        if (input?.Trim().Length != 0)
            input = input?.Trim();
        if (input?.Length >= 1)
        {
            var lastChar = input.ToLower()[^1];
            if (!Alphabet.Contains(lastChar))
                lastChar = ' ';
            input = lastChar.ToString();
        }
        else
        {
            input = " ";
        }
        return input[0];
    }
}