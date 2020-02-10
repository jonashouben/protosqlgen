using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using ProtoSqlGen.MariaDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProtoSqlGen.SqlServer;

namespace ProtoSqlGen.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register(nameof(ConnectionString), typeof(string), typeof(MainWindow));
		private static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register(nameof(OutputText), typeof(string), typeof(MainWindow));

		private Database Database;

		public string ConnectionString
		{
			get => (string)GetValue(ConnectionStringProperty);
			set => SetValue(ConnectionStringProperty, value);
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
			OutputText = "";
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			string selected = (dbSelection.SelectedItem as ComboBoxItem)?.Content as string;

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
				OutputText = (await Database.GetDatabases().FirstOrDefaultAsync())?.GetProto();
			}
		}
	}
}
