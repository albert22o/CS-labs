﻿using System.Windows;
using System.Windows.Controls;

namespace TaskNo3
{
    /// <summary>
    /// Этот класс отвечает за обработку пользовательского интерфейса окна приложения.
    /// Он позволяет выбирать планеты из списка и отображать их описания.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> _planets = new(8);
        private readonly List<string> _planetsDescription = new(8);

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация списка названий планет
            _planets.Add("Меркурий");
            _planets.Add("Венера");
            _planets.Add("Земля");
            _planets.Add("Марс");
            _planets.Add("Юпитер");
            _planets.Add("Сатурн");
            _planets.Add("Уран");
            _planets.Add("Нептун");

            PlanetsList.ItemsSource = _planets;

            // Инициализация списка описаний планет
            _planetsDescription.Add(
                "Меркурий находится ближе всех к Солнцу и является наименьшей планетой нашей системы. " +
                "Поверхность покрыта кратерами, а разреженная атмосфера состоит из атомов, выбитых солнечным ветром. " +
                "Его железное ядро и тонкая кора указывают на уникальную историю формирования.");
            _planetsDescription.Add(
                "Венера схожа по размеру с Землей, но обладает плотной атмосферой из углекислого газа, " +
                "создающей экстремальный парниковый эффект. Это делает Венеру самой горячей планетой в Солнечной системе.");
            _planetsDescription.Add(
                "Земля уникальна среди планет благодаря наличию жидкой воды, тектонике плит и кислородной атмосфере. " +
                "Ее спутник Луна оказывает значительное влияние на жизнь и климат нашей планеты.");
            _planetsDescription.Add(
                "Марс известен своей красной поверхностью, вулканами и глубокими каньонами. " +
                "Хотя его атмосфера разрежена, на планете могли существовать условия для жизни в прошлом.");
            _planetsDescription.Add(
                "Юпитер — самая большая планета, состоящая в основном из водорода и гелия. " +
                "Его атмосфера украшена мощными бурями, включая знаменитое Большое Красное Пятно, а спутники планеты поражают разнообразием.");
            _planetsDescription.Add(
                "Сатурн выделяется своими кольцами, состоящими из льда и пыли. " +
                "Его крупнейший спутник Титан имеет плотную атмосферу, что делает его объектом интереса ученых.");
            _planetsDescription.Add(
                "Уран отличается наклоном оси вращения, из-за чего он как бы катится по своей орбите. " +
                "Планета излучает мало тепла и имеет холодное ядро по сравнению с другими гигантами.");
            _planetsDescription.Add(
                "Нептун — самая дальняя планета, известная своими сильными ветрами и голубым оттенком, вызванным метаном в атмосфере. " +
                "Его спутник Тритон уникален своим обратным орбитальным движением.");
        }

        private void PlanetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (sender as ListBox)!.SelectedIndex;
            PlanetDescription.Text = _planetsDescription[index]; // Отображение описания выбранной планеты
        }
    }
}
