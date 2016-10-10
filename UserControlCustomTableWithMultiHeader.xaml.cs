using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ASRVGModel;
using BaseElementLibrary;

namespace ASRVGMainApp.WPF
{
    /// <summary>
    /// Interaction logic for UserControlCustomTableWithMultiHeader.xaml
    /// </summary>
    public partial class UserControlCustomTableWithMultiHeader : UserControl
    {
        private int _frozenColumnCount;

        /// <summary>
        /// Количество замороженных столбцов таблицы, начиная слева.
        /// Следует использовать это свойство, т.к. аналогичное свойство таблицы (MainTable.FrozenColumnCount) багует =)
        /// </summary>
        public int FrozenColumnCount 
        { 
            get 
            {
                return _frozenColumnCount;
            }
            set
            {
                _frozenColumnCount = value;
                MainTable.FrozenColumnCount = value;
            }
        }

        public UserControlCustomTableWithMultiHeader()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Создает таблицу и мульти заголовок из шаблона
        /// </summary>
        /// <param name="columnsWidht">Набор ширин каждой колонки</param>
        /// <param name="rowsHeight">Набор высот каждой строки</param>
        /// <param name="templates">Набор шаблонов</param>
        /// <param name="dataBindings">Набор привязок данных</param>
        /// <param name="dataSource">Источник данных для таблицы</param>
        public void RefreshTable(float[] columnsWidht, float[] rowsHeight,  List<CustomHeaderTemplate> templates, List<string> dataBindings, IEnumerable dataSource)
        {
            //Чистим заголовок и таблицу
            MainTable.Columns.Clear();
            CustomHeader.ColumnDefinitions.Clear();
            CustomHeader.RowDefinitions.Clear();
            CustomHeader.Children.Clear();
            
            //Чистим замороженный заголовок
            FrozenHeader.ColumnDefinitions.Clear();
            FrozenHeader.RowDefinitions.Clear();
            FrozenHeader.Children.Clear();

            var columnsCount = columnsWidht.Count();
            var rowsCount = rowsHeight.Count();

            MainTable.ItemsSource = dataSource;

            for (int c = 0; c < columnsCount; c++)
            {
                //Создать колонку таблицы
                var column = new DataGridTextColumn
                {
                    Header = " ",
                    Binding = new Binding(dataBindings[c]),
                    Width = columnsWidht[c]
                };

                //Создать колонку для заголовка
                var headerColumn = new ColumnDefinition();

                //Создать объект привязки размеров для колонки таблицы
                var headerBinding = new Binding { Source = column, Path = new PropertyPath("ActualWidth") };

                //Добавить колонки в таблицу и заголовок
                MainTable.Columns.Add(column);
                CustomHeader.ColumnDefinitions.Add(headerColumn);

                //Связать размеры колонок таблицы и заголовка
                BindingOperations.SetBinding(headerColumn, ColumnDefinition.WidthProperty, headerBinding);

                //Создаем замороженную колонку (пока прозрачную)
                var frozenColumn = new ColumnDefinition();
                BindingOperations.SetBinding(frozenColumn, ColumnDefinition.WidthProperty, headerBinding);
                FrozenHeader.ColumnDefinitions.Add(frozenColumn);
            }
            
            for (int r = 0; r < rowsCount; r++)
            {
                //Создать строку для заголовка
                var headerRow = new RowDefinition
                {
                    Height = new GridLength(rowsHeight[r])
                };

                //Добавить строку в заголовок
                CustomHeader.RowDefinitions.Add(headerRow);

                //Создаем замороженную строку (пока прозрачную)
                var frozenRow = new RowDefinition
                {
                    Height = new GridLength(rowsHeight[r])
                };
                FrozenHeader.RowDefinitions.Add(frozenRow);
            }

            //Объединить ячейки согласно шаблону
            foreach (var template in templates)
            {
                //Создать новую границу с текстовым элементом
                var border = new Border
                {
                    Child = new TextBlock 
                    { 
                        Text = template.Text, 
                        TextWrapping = TextWrapping.Wrap, 
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    }
                };

                //Установить положение
                border.SetValue(Grid.RowProperty, template.Row);
                border.SetValue(Grid.ColumnProperty, template.Column);

                //Установить объединение
                border.SetValue(Grid.RowSpanProperty, template.RowSpan);
                border.SetValue(Grid.ColumnSpanProperty, template.ColumnSpan);

                CustomHeader.Children.Add(border);

                //Обводим и заливаем замороженные ячейки
                if (template.Column < FrozenColumnCount)
                {
                    var frozenBorder = new Border
                    {
                        Child = new Label { Content = template.Text }
                    };

                    //Установить положение
                    frozenBorder.SetValue(Grid.RowProperty, template.Row);
                    frozenBorder.SetValue(Grid.ColumnProperty, template.Column);

                    //Установить объединение
                    frozenBorder.SetValue(Grid.RowSpanProperty, template.RowSpan);
                    frozenBorder.SetValue(Grid.ColumnSpanProperty, template.ColumnSpan);

                    FrozenHeader.Children.Add(frozenBorder);
                }
            }
        }

        /// <summary>
        /// Обработчик события изменения положения скрола.
        /// Синхронизирует соложения скролов заголовка и таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainTable_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var value = e.HorizontalOffset;

            HeaderScroll.ScrollToHorizontalOffset(value);
        }
    }



    /// <summary>
    /// Класс шаблона ячейки заголовка
    /// </summary>
    public class CustomHeaderTemplate
    {
        /// <summary>
        /// Номер строки
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// Номер столбца
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// Высота в строках
        /// </summary>
        public int RowSpan { get; set; }
        /// <summary>
        /// Ширина в колонках
        /// </summary>
        public int ColumnSpan { get; set; }
        /// <summary>
        /// Текст внитри ячейки
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <param name="row">Номер строки</param>
        /// <param name="column">Номер столбца</param>
        /// <param name="rowSpan">Высота в строках</param>
        /// <param name="columnSpan">Ширина в колонках</param>
        /// <param name="text">Текст внитри ячейки</param>
        public CustomHeaderTemplate(int row, int column, int rowSpan, int columnSpan, string text)
        {
            Row = row;
            Column = column;
            RowSpan = rowSpan;
            ColumnSpan = columnSpan;
            Text = text;
        }
    }
}
