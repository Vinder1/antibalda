using AntiBaldaGame.Models;
using NUnit.Framework;
using System;

namespace AntiBaldaGame.Tests
{
    [TestFixture]
    public class StringFormatterTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        [Description("Возвращает последний символ строки в нижнем регистре")]
        public void LeaveOneCharacter_WithValidString_ReturnsLastLowercaseChar()
        {
            // Arrange
            string input = "Тест";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo('т'));
        }

        [Test]
        [Description("Возвращает пробел для символа не из алфавита")]
        public void LeaveOneCharacter_WithNonAlphabetChar_ReturnsSpace()
        {
            // Arrange
            string input = "test!";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo(' '));
        }

        [Test]
        [Description("Возвращает пробел для пустой строки")]
        public void LeaveOneCharacter_WithEmptyString_ReturnsSpace()
        {
            // Arrange
            string input = "";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo(' '));
        }

        [Test]
        [Description("Возвращает пробел для строки из пробелов")]
        public void LeaveOneCharacter_WithWhitespaceString_ReturnsSpace()
        {
            // Arrange
            string input = "   ";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo(' '));
        }

        [Test]
        [Description("Возвращает пробел для null")]
        public void LeaveOneCharacter_WithNullInput_ReturnsSpace()
        {
            // Arrange
            string? input = null;

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo(' '));
        }

        [Test]
        [Description("Корректно обрабатывает строку с одним символом")]
        public void LeaveOneCharacter_WithSingleChar_ReturnsThatChar()
        {
            // Arrange
            string input = "Я";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo('я'));
        }

        [Test]
        [Description("Обрезает пробелы по краям строки")]
        public void LeaveOneCharacter_TrimsInputString()
        {
            // Arrange
            string input = "  слово  ";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo('о'));
        }

        [Test]
        [Description("Корректно обрабатывает символы верхнего регистра")]
        public void LeaveOneCharacter_ConvertsToLowercase()
        {
            // Arrange
            string input = "ПрИвЕт";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo('т'));
        }

        [Test]
        [Description("Возвращает пробел для цифр")]
        public void LeaveOneCharacter_WithNumbers_ReturnsSpace()
        {
            // Arrange
            string input = "123";

            // Act
            char result = StringFormatter.LeaveOneCharacter(input);

            // Assert
            Assert.That(result, Is.EqualTo(' '));
        }
    }

    
}