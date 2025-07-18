using AntiBaldaGame.Models;
using AntiBaldaGame.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NUnit.Framework;
using System;
using System.Reflection;

namespace AntiBaldaGame.Tests
{
    [TestFixture]
    public class LettersGridTests
    {
        private LettersGrid _grid;
        private GameWindowViewModel _viewModel;
        private PropertyInfo _modeProperty;
        private const int TestGridSize = 5;

        [SetUp]
        public void Setup()
        {
            try
            {
                // Инициализация тестовых настроек
                InitializeTestSettings();

                // Создание и инициализация сетки
                _grid = new LettersGrid();
                InitializeGridCells();

                // Создание и настройка ViewModel
                _viewModel = new GameWindowViewModel();
                InitializeViewModel();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test initialization failed: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void InitializeTestSettings()
        {
            // Инициализация Settings если используется
            if (Settings.Instance == null)
            {
                Settings.Instance = new Settings { GridSize = TestGridSize };
            }
        }

        private void InitializeGridCells()
        {
            try
            {
                for (int i = 0; i < TestGridSize; i++)
                {
                    for (int j = 0; j < TestGridSize; j++)
                    {
                        var button = _grid.Get(i, j);
                        if (button == null)
                        {
                            throw new InvalidOperationException($"Button at ({i},{j}) is null");
                        }
                        button.Letter = ' ';
                        button.IsSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Grid initialization failed", ex);
            }
        }

        private void InitializeViewModel()
        {
            // Настройка режима через рефлексию
            _modeProperty = typeof(GameWindowViewModel).GetProperty("Mode");
            if (_modeProperty == null || !_modeProperty.CanWrite)
            {
                throw new InvalidOperationException("Cannot access Mode property");
            }
            _modeProperty.SetValue(_viewModel, GameWindowViewModel.GameMode.LetterChoosing);

        }

   
    }

    #region Test Helpers
    public class Settings
    {
        public static Settings Instance { get; set; }
        public int GridSize { get; set; }
    }

    public class CustomColors
    {
        public static string White = "White";
        public static string Default = "Default";
    }
    #endregion
}