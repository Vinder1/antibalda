using AntiBaldaGame.Models;
using NUnit.Framework;
using System;

namespace AntiBaldaGame.Tests.Models
{
    [TestFixture]
    public class LetterSequenceTests
    {
        private LetterButton _buttonA;
        private LetterButton _buttonB;
        private LetterButton _buttonC;
        private CoordinatedLetterButton _coordButton1;
        private CoordinatedLetterButton _coordButton2;
        private CoordinatedLetterButton _coordButton3;

        [SetUp]
        public void Setup()
        {
            _buttonA = new LetterButton { Letter = 'A' };
            _buttonB = new LetterButton { Letter = 'B' };
            _buttonC = new LetterButton { Letter = 'C' };

            _coordButton1 = new CoordinatedLetterButton(_buttonA, 0, 0);
            _coordButton2 = new CoordinatedLetterButton(_buttonB, 0, 1);
            _coordButton3 = new CoordinatedLetterButton(_buttonC, 1, 0);
        }

        // ===================== TryAdd Tests =====================

        [Test]
        [Description("Добавление соседней буквы справа - должно добавиться")]
        public void TryAdd_WhenAddingRightNeighbor_ReturnsTrue()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            var rightNeighbor = new CoordinatedLetterButton(_buttonB, 0, 1);

            // Act
            var result = sequence.TryAdd(rightNeighbor);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [Description("Добавление несоседней буквы - должно вернуть false")]
        public void TryAdd_WhenAddingNonNeighbor_ReturnsFalse()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            var nonNeighbor = new CoordinatedLetterButton(_buttonB, 2, 2);

            // Act
            var result = sequence.TryAdd(nonNeighbor);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        [Description("Добавление соседней буквы снизу - должно добавиться")]
        public void TryAdd_WhenAddingBottomNeighbor_ReturnsTrue()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            var bottomNeighbor = new CoordinatedLetterButton(_buttonB, 1, 0);

            // Act
            var result = sequence.TryAdd(bottomNeighbor);

            // Assert
            Assert.That(result, Is.True);
        }

        // ===================== TryRemoveFirstOrLast Tests =====================

        [Test]
        [Description("Удаление первой буквы - должно удалиться")]
        public void TryRemoveFirstOrLast_WhenRemovingFirstItem_ReturnsTrue()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            sequence.TryAdd(_coordButton2);

            // Act
            var result = sequence.TryRemoveFirstOrLast(_coordButton1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [Description("Удаление последней буквы - должно удалиться")]
        public void TryRemoveFirstOrLast_WhenRemovingLastItem_ReturnsTrue()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            sequence.TryAdd(_coordButton2);

            // Act
            var result = sequence.TryRemoveFirstOrLast(_coordButton2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [Description("Удаление средней буквы - должно вернуть false")]
        public void TryRemoveFirstOrLast_WhenRemovingMiddleItem_ReturnsFalse()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            sequence.TryAdd(_coordButton2);
            sequence.TryAdd(_coordButton3);

            // Act
            var result = sequence.TryRemoveFirstOrLast(_coordButton2);

            // Assert
            Assert.That(result, Is.False);
        }

        // ===================== GetWord Tests =====================

        [Test]
        [Description("Получение слова из одной буквы - должен вернуть эту букву")]
        public void GetWord_WithSingleLetter_ReturnsCorrectLetter()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);

            // Act
            var word = sequence.GetWord();

            // Assert
            Assert.That(word, Is.EqualTo("A"));
        }

        [Test]
        [Description("Получение слова из нескольких букв - должен вернуть правильное слово")]
        public void GetWord_WithMultipleLetters_ReturnsCorrectWord()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            sequence.TryAdd(_coordButton2);
            sequence.TryAdd(_coordButton3);

            // Act
            var word = sequence.GetWord();

            // Assert
            Assert.That(word, Is.EqualTo("ABC"));
        }

        [Test]
        [Description("Получение слова после удаления буквы - должен вернуть правильное слово")]
        public void GetWord_AfterRemoval_ReturnsCorrectWord()
        {
            // Arrange
            var sequence = new LetterSequence(_coordButton1);
            sequence.TryAdd(_coordButton2);
            sequence.TryAdd(_coordButton3);
            sequence.TryRemoveFirstOrLast(_coordButton3);

            // Act
            var word = sequence.GetWord();

            // Assert
            Assert.That(word, Is.EqualTo("AB"));
        }
    }
}