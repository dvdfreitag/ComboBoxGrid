using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Controls
{
    public partial class CheckBoxGrid : UserControl
    {
        private ObservableCollection<ObservableCollection<CheckBox>> CheckBoxes = new ObservableCollection<ObservableCollection<CheckBox>>();
        private ObservableCollection<ObservableCollection<bool>> _states = new ObservableCollection<ObservableCollection<bool>>();

        public ObservableCollection<ObservableCollection<bool>> States
        {
            get
            {
                return _states;
            }

            set
            {
                _states = value;
            }
        }

        private ObservableCollection<TextBlock> RowLabels = new ObservableCollection<TextBlock>();
        private ObservableCollection<TextBlock> ColumnLabels = new ObservableCollection<TextBlock>();

        private int _columns = 0;
        private int _rows = 0;

        private bool _showLabels = true;

        private Thickness _spacing = new Thickness(2);

        public static DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(CheckBoxGrid), 
            new PropertyMetadata(0, new PropertyChangedCallback(OnColumnsChanged)));
        
        private static void OnColumnsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var container = sender as CheckBoxGrid;
            if (container == null) return;

            var grid = container.Container;
            if (grid == null) return;

            var start = (int)e.OldValue;
            var end = (int)e.NewValue;

            container._columns = end;

            if (start < end)
            {
                for (var i = (start + 1); i <= end; i++)
                {
                    container.CheckBoxes.Add(new ObservableCollection<CheckBox>());
                    container.States.Add(new ObservableCollection<bool>());

                    var column = new ColumnDefinition();
                    column.Width = new GridLength(1, GridUnitType.Auto);
                    grid.ColumnDefinitions.Add(column);

                    var label = new TextBlock();
                    label.Text = i.ToString();
                    label.SetValue(Grid.ColumnProperty, i);
                    label.SetValue(Grid.RowProperty, 0);
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                    label.VerticalAlignment = VerticalAlignment.Bottom;
                    container.ColumnLabels.Add(label);
                    grid.Children.Add(label);
                }
            }

            if (container._rows == 0) return;
            if (start == end) return;
            
            if (start > end)
            {
                for (var i = (start - 1); i > (end - 1); i--)
                {
                    for (var j = (container._rows - 1); j >= 0; j--)
                    {
                        var box = container.CheckBoxes[i][j];
                        BindingOperations.ClearBinding(box, CheckBox.IsCheckedProperty);
                        grid.Children.Remove(box);
                    }

                    container.CheckBoxes.RemoveAt(i);
                    container.States.RemoveAt(i);
                    grid.ColumnDefinitions.RemoveAt(i);

                    grid.Children.Remove(container.ColumnLabels[i]);
                    container.ColumnLabels.RemoveAt(i);
                }

                if (end == 0)
                {
                    foreach (var label in container.RowLabels)
                    {
                        label.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                for (var i = (start + 1); i <= end; i++)
                {
                    for (var j = 1; j <= container._rows; j++)
                    {
                        container.States[i - 1].Add(false);

                        var box = new CheckBox();
                        box.SetValue(Grid.ColumnProperty, i);
                        box.SetValue(Grid.RowProperty, j);
                        box.DataContext = container;
                        box.Margin = container._spacing;

                        box.SetBinding(CheckBox.IsCheckedProperty, new Binding("States[" + (i - 1) + "][" + (j - 1) + "]")
                        {
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });

                        container.CheckBoxes[i - 1].Add(box);
                        grid.Children.Add(box);
                    }
                }

                if (start == 0)
                {
                    foreach (var label in container.RowLabels)
                    {
                        label.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public int Columns
        {
            get
            {
                return _columns;
            }

            set
            {
                if (value < 0) return;

                OnColumnsChanged(this, new DependencyPropertyChangedEventArgs(ColumnsProperty, _columns, value));
            }
        }

        public static DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(CheckBoxGrid), 
            new PropertyMetadata(0, new PropertyChangedCallback(OnRowsChanged)));

        private static void OnRowsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var container = sender as CheckBoxGrid;
            if (container == null) return;

            var grid = container.Container;
            if (grid == null) return;

            var start = (int)e.OldValue;
            var end = (int)e.NewValue;
            
            container._rows = end;

            if (start < end)
            {
                for (var i = (start + 1); i <= end; i++)
                {
                    var row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Auto);
                    grid.RowDefinitions.Add(row);

                    var label = new TextBlock();
                    label.Text = i.ToString();
                    label.SetValue(Grid.ColumnProperty, 0);
                    label.SetValue(Grid.RowProperty, i);
                    label.HorizontalAlignment = HorizontalAlignment.Right;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    container.RowLabels.Add(label);
                    grid.Children.Add(label);
                }
            }

            if (container._columns == 0) return;
            if (start == end) return;
            
            if (start > end)
            {
                for (var i = (container._columns - 1); i >= 0; i--)
                {
                    for (var j = (start - 1); j > (end - 1); j--)
                    {
                        var box = container.CheckBoxes[i][j];
                        BindingOperations.ClearBinding(box, CheckBox.IsCheckedProperty);
                        grid.Children.Remove(box);
                        container.CheckBoxes[i].Remove(box);
                        container.States[i].RemoveAt(j);
                    }
                }

                for (var i = start; i > end; i--)
                {
                    grid.Children.Remove(container.RowLabels[i - 1]);
                    container.RowLabels.RemoveAt(i - 1);
                    grid.RowDefinitions.RemoveAt(i);
                }

                if (end == 0)
                {
                    foreach (var label in container.ColumnLabels)
                    {
                        label.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                for (var i = 1; i <= container._columns; i++)
                {
                    for (var j = (start + 1); j <= end; j++)
                    {
                        container.States[i - 1].Add(false);

                        var box = new CheckBox();
                        box.SetValue(Grid.ColumnProperty, i);
                        box.SetValue(Grid.RowProperty, j);
                        box.DataContext = container;
                        box.Margin = container._spacing;
                        box.Checked += container.OnCheckBoxChecked;
                        box.Unchecked += container.OnCheckBoxChecked;

                        box.SetBinding(CheckBox.IsCheckedProperty, new Binding("States[" + (i - 1) + "][" + (j - 1) + "]")
                        {
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });

                        container.CheckBoxes[i - 1].Add(box);
                        grid.Children.Add(box);
                    }
                }

                if (start == 0)
                {
                    foreach (var label in container.ColumnLabels)
                    {
                        label.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public int Rows
        {
            get
            {
                return _rows;
            }

            set
            {
                if (value < 0) return;

                OnRowsChanged(this, new DependencyPropertyChangedEventArgs(RowsProperty, _rows, value));
            }
        }

        public static DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(CheckBoxGrid), 
            new PropertyMetadata(true, new PropertyChangedCallback(OnShowLabelsChanged)));

        private static void OnShowLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var container = sender as CheckBoxGrid;
            if (container == null) return;

            var grid = container.Container;
            if (grid == null) return;

            if (e.NewValue == e.OldValue) return;

            container._showLabels = (bool)e.NewValue;

            if (container._showLabels)
            {
                grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                grid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                grid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Pixel);
                grid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        public bool ShowLabels
        {
            get
            {
                return _showLabels;
            }

            set
            {
                OnShowLabelsChanged(this, new DependencyPropertyChangedEventArgs(ShowLabelsProperty, _showLabels, value));
            }
        }

        public static DependencyProperty SpacingProperty = DependencyProperty.Register("Spacing", typeof(Thickness), typeof(CheckBoxGrid),
            new PropertyMetadata(new Thickness(2), new PropertyChangedCallback(OnSpacingChanged)));

        private static void OnSpacingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var container = sender as CheckBoxGrid;
            if (container == null) return;

            container._spacing = (Thickness)e.NewValue;

            foreach (var columns in container.CheckBoxes)
            {
                foreach (var box in columns)
                {
                    box.Margin = container._spacing;
                }
            }
        }

        public Thickness Spacing
        {
            get
            {
                return _spacing;
            }

            set
            {
                OnSpacingChanged(this, new DependencyPropertyChangedEventArgs(SpacingProperty, _spacing, value));
            }
        }

        public event EventHandler<CheckBoxGridCheckedEventArgs> Checked;

        public void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var box = sender as CheckBox;
            if (box == null) return;

            var column = (int)box.GetValue(Grid.ColumnProperty) - 1;
            var row = (int)box.GetValue(Grid.RowProperty) - 1;

            var handler = Checked;

            if (handler != null)
            {
                handler(this, new CheckBoxGridCheckedEventArgs(column, row, (box.IsChecked ?? false)));
            }
        }

        public CheckBoxGrid()
        {
            InitializeComponent();
        }
    }
}
