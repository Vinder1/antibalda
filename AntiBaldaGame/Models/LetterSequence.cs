using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiBaldaGame.Models;

public record CoordinatedLetterButton(LetterButton Button, int Y, int X);

public class LetterSequence
{
    private readonly LinkedList<CoordinatedLetterButton> sequence = [];

    public bool TryAdd(CoordinatedLetterButton button)
    {
        var first = sequence.First?.Value!;
        var last = sequence.Last?.Value!;

        if (Math.Abs(first.X - button.X) == 0 && Math.Abs(first.Y - button.Y) == 1
        || Math.Abs(first.X - button.X) == 1 && Math.Abs(first.Y - button.Y) == 0)
        {
            sequence.AddFirst(button);
            return true;
        }
        if (Math.Abs(last.X - button.X) == 0 && Math.Abs(last.Y - button.Y) == 1
        || Math.Abs(last.X - button.X) == 1 && Math.Abs(last.Y - button.Y) == 0)
        {
            sequence.AddLast(button);
            return true;
        }
        return false;
    }

    public bool TryRemoveFirstOrLast(CoordinatedLetterButton button)
    {
        if (sequence.First!.Value == button)
        {
            sequence.RemoveFirst();
            return true;
        }
        else if (sequence.Last!.Value == button)
        {
            sequence.RemoveLast();
            return true;
        }
        return false;
    }

    public string GetWord()
    {
        return sequence.Aggregate(
            new StringBuilder(),
            (sb, button) => sb.Append(button.Button.Letter)
        ).ToString();
    }

    public LetterSequence(CoordinatedLetterButton button)
    {
        sequence.AddLast(button);
    }
}