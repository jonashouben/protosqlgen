using System.Collections.Generic;
using System.Data.SqlClient;
using ProtoSqlGen.MariaDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProtoSqlGen.SqlServer;
using MySqlConnector;

namespace ProtoSqlGen.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register(nameof(ConnectionString), typeof(string), typeof(MainWindow));
		private static readonly DependencyProperty DatabasesProperty = DependencyProperty.Register(nameof(Databases), typeof(IReadOnlyCollection<string>), typeof(MainWindow));
		private static readonly DependencyProperty TablesProperty = DependencyProperty.Register(nameof(Tables), typeof(IReadOnlyCollection<string>), typeof(MainWindow));
		private static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register(nameof(OutputText), typeof(string), typeof(MainWindow));

		private Database Database;

		public string ConnectionString
		{
			get => (string)GetValue(ConnectionStringProperty);
			set => SetValue(ConnectionStringProperty, value);
		}

		public IReadOnlyCollection<string> Databases
		{
			get => (IReadOnlyCollection<string>) GetValue(DatabasesProperty);
			set => SetValue(DatabasesProperty, value);
		}

		public IReadOnlyCollection<string> Tables
		{
			get => (IReadOnlyCollection<string>) GetValue(TablesProperty);
			set => SetValue(TablesProperty, value);
		}

		public string OutputText
		{
			get => (string)GetValue(OutputTextProperty);
			set => SetValue(OutputTextProperty, value);
		}

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			ConnectionString = "";
			Databases = new List<string>();
			Tables = new List<string>();
			OutputText = "";
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			string selected = (connectorSelection.SelectedItem as ComboBoxItem)?.Content as string;

			switch (selected)
			{
				case "MariaDB":
					Database = new MariaDbDatabase(new MySqlConnection(ConnectionString));
					break;
				case "SQL Server":
					Database = new SqlServerDatabase(new SqlConnection(ConnectionString));
					break;
				default:
					Database = null;
					break;
			}

			if (Database != null)
			{
				Databases = (await Database.GetDatabaseNames()).OrderBy(row => row).ToList();
			}
		}

		private async void Button_Click_1(object sender, RoutedEventArgs e)
		{
			if (Database != null && dbSelection.SelectedItem is string selectedDatabase && tableSelection.SelectedItem is string selectedTable)
			{
				OutputText = new ProtoFile(selectedDatabase, new []{ await Database.GetTable(selectedDatabase, selectedTable) }).GetProto();
			}
		}

		private async void dbSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Database != null && dbSelection.SelectedItem is string selected)
			{
				Tables = (await Database.GetTableNames(selected)).OrderBy(row => row).ToList();
			}
		}
	}
}
