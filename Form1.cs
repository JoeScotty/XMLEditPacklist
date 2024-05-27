

//*******************************************************************************
// ToDo:
// ComboBox: kann nicht nichts auswählen (Um alle Kategorien anzuzeigen).
// nach Auswahl category list erscheint nur eine Zeile anstatt alle mit der Category
// Text oberhablb der Combo Box als Beispiel

using System;
using System.Data;
using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Linq;


namespace ShowXML
{
    public partial class Form1 : Form
    {
        // Other members and methods...

        private DataGridView dataGridView1 = new DataGridView();
        private Button buttonOpenFile = new Button();
        private Button buttonSave = new Button();
        private Button buttonSaveAs = new Button();

        // Define a variable to store the file path
        private string? filePath;

        private ComboBox? comboBoxColumns;
        private ComboBox? comboBoxCategories;  // Add this line
        private ListBox? listBoxContent;
        private DataTable? originalDataTable;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Code for initializing components

            // dataGridView1
            this.dataGridView1.Location = new System.Drawing.Point(10, 80);
            this.dataGridView1.Size = new System.Drawing.Size(800, 400);

            // buttonOpenFile
            this.buttonOpenFile.Location = new System.Drawing.Point(10, 30);
            this.buttonOpenFile.Size = new System.Drawing.Size(100, 30);
            this.buttonOpenFile.Text = "Open File";

            // buttonSave
            this.buttonSave.Location = new System.Drawing.Point(140, 30);
            this.buttonSave.Size = new System.Drawing.Size(100, 30);
            this.buttonSave.Text = "Save";

            // buttonSaveAs
            this.buttonSaveAs.Location = new System.Drawing.Point(270, 30);
            this.buttonSaveAs.Size = new System.Drawing.Size(100, 30);
            this.buttonSaveAs.Text = "Save As";

            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonSaveAs);

            this.ClientSize = new System.Drawing.Size(830, 630);
            this.Text = "XML Editor";
            this.Load += new EventHandler(Form1_Load);

            this.ResumeLayout(false);

            this.buttonOpenFile.Click += new EventHandler(buttonOpenFile_Click);
            this.buttonSave.Click += new EventHandler(buttonSave_Click);
            this.buttonSaveAs.Click += new EventHandler(buttonSaveAs_Click);
            this.Resize += new EventHandler(MainForm_Resize); // resize main windows

            // comboBoxColumns
            this.comboBoxColumns = new System.Windows.Forms.ComboBox();
            this.comboBoxColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColumns.FormattingEnabled = true;
            this.comboBoxColumns.Location = new System.Drawing.Point(270, 510); // Adjust the location
            this.comboBoxColumns.Name = "comboBoxColumns";
            this.comboBoxColumns.Size = new System.Drawing.Size(100, 24); // Adjust the size
            this.comboBoxColumns.TabIndex = 0;
            this.Controls.Add(this.comboBoxColumns);
            this.comboBoxColumns.SelectedIndexChanged += new EventHandler(comboBoxColumns_SelectedIndexChanged);

            // listBoxContent
            this.listBoxContent = new System.Windows.Forms.ListBox();
            this.listBoxContent.FormattingEnabled = true;
            this.listBoxContent.ItemHeight = 16;
            this.listBoxContent.Location = new System.Drawing.Point(500, 510); // Adjust the location
            this.listBoxContent.Name = "listBoxContent";
            this.listBoxContent.Size = new System.Drawing.Size(200, 100); // Adjust the size
            this.listBoxContent.TabIndex = 1;
            this.Controls.Add(this.listBoxContent);
            this.listBoxContent.SelectedIndexChanged += new EventHandler(listBoxContent_SelectedIndexChanged);

            // comboBoxCategories
            this.comboBoxCategories = new System.Windows.Forms.ComboBox();
            this.comboBoxCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCategories.FormattingEnabled = true;
            this.comboBoxCategories.Location = new System.Drawing.Point(10, 510); // Adjust the location
            this.comboBoxCategories.Name = "comboBoxCategories";
            this.comboBoxCategories.Size = new System.Drawing.Size(100, 24); // Adjust the size
            this.comboBoxCategories.TabIndex = 2;
            this.Controls.Add(this.comboBoxCategories);
            this.comboBoxCategories.SelectedIndexChanged += new EventHandler(comboBoxCategories_SelectedIndexChanged);
            
            this.listBoxContent.SelectedIndexChanged += new EventHandler(listBoxContent_SelectedIndexChanged);



        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            // Adjust the size and position of the DataGridView
            // For example:
            dataGridView1.Size = new Size(this.ClientSize.Width - 30, this.ClientSize.Height - 230);
            dataGridView1.Location = new Point(10, 80);

            if (this.listBoxContent != null)
            {
                this.listBoxContent.Location = new System.Drawing.Point(500, this.ClientSize.Height - 120);
            }

            if (this.comboBoxColumns != null)
            {
                this.comboBoxColumns.Location = new System.Drawing.Point(270, this.ClientSize.Height - 120);
            }

            if (this.comboBoxCategories != null)
            {
                this.comboBoxCategories.Location = new System.Drawing.Point(10, this.ClientSize.Height - 120);
            }
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Code to handle form load event
        }

        private void buttonOpenFile_Click(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Open XML File";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName; // Store the file path
                LoadXmlFile(filePath);
            }
        }

        private void buttonSave_Click(object? sender, EventArgs e)
        {
            if (filePath != null)
            {
                SaveToXml(filePath);
            }
            else
            {
                MessageBox.Show("File path is null. Please make sure a file is loaded or saved.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonSaveAs_Click(object? sender, EventArgs e)
        {
            // Check if the DataGridView is bound to any data
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("No XML file is currently loaded. Please open a file first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Prompt the user to select a location and filename using the standard Windows file dialog
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML Files|*.xml";
            saveFileDialog1.Title = "Save XML File As";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName; // Store the file path

                // Proceed with saving using the filePath
                SaveToXml(filePath);
            }
        }

        private void comboBoxColumns_SelectedIndexChanged(object? sender, EventArgs e)
        {
            PopulateContentList();
        }

        private void SaveToXml(string filePath)
        {
            try
            {
                // Check if the DataGridView is bound to any data
                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("No XML file is currently loaded. Please open a file first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Create XML content as string
                StringBuilder xmlContent = new StringBuilder();
                xmlContent.AppendLine("\t<packingList>");

                // Get distinct categories
                var categories = dataGridView1.Rows.Cast<DataGridViewRow>()
                    .Where(row => !row.IsNewRow)
                    .Select(row => row.Cells["Category"].Value?.ToString())
                    .Where(category => category != null) // Filter out null categories
                    .Distinct()
                    .ToList();

                // Iterate through categories
                foreach (var category in categories)
                {
                    // Skip processing if category is null
                    if (category == null)
                    {
                        continue;
                    }

                    xmlContent.AppendLine($"\t<category name='{category}'>");

                    // Iterate through items in the category
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow && row.Cells["Category"].Value?.ToString() == category)
                        {
                            // Encode special characters in item name before adding to XML
                            string itemName = EncodeSpecialCharacters(row.Cells["Name"].Value?.ToString() ?? "");

                            xmlContent.AppendLine($"\t\t<item name='{itemName}'" +
                                                $" quantity='{row.Cells["Quantity"].Value?.ToString()?.Replace("\"", "'")}'" +
                                                $" checked='{row.Cells["Checked"].Value?.ToString()?.Replace("\"", "'")}'" +
                                                $" isActive='{row.Cells["IsActive"].Value?.ToString()?.Replace("\"", "'")}'" +
                                                $" location='{row.Cells["Location"].Value?.ToString()?.Replace("\"", "'")}'" +
                                                $" luggage='{row.Cells["Luggage"].Value?.ToString()?.Replace("\"", "'")}'/>");
                        }
                    }

                    xmlContent.AppendLine("\t</category>");
                }

                xmlContent.AppendLine("\t</packingList>");

                // Save the XML content to the selected file
                System.IO.File.WriteAllText(filePath, xmlContent.ToString());
                MessageBox.Show("File saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving XML file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadXmlFile(string filePath)
        {
            try
            {
                // Load XML file
                XDocument doc = XDocument.Load(filePath);

                // Parse XML data and bind to DataGridView
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Category");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("Location");
                dt.Columns.Add("Checked"); // Add Checked column
                dt.Columns.Add("IsActive"); // Add IsActive column
                dt.Columns.Add("Luggage"); // Add Luggage column

                foreach (XElement item in doc.Descendants("item"))
                {
                    string? category = item.Parent?.Attribute("name")?.Value ?? "";
                    dt.Rows.Add(
                        item.Attribute("name")?.Value ?? "",
                        category,
                        item.Attribute("quantity")?.Value ?? "",
                        item.Attribute("location")?.Value ?? "",
                        item.Attribute("checked")?.Value ?? "", // Add Checked value
                        item.Attribute("isActive")?.Value ?? "", // Add IsActive value
                        item.Attribute("luggage")?.Value ?? "" // Add Luggage value
                    );
                }

                originalDataTable = dt.Copy(); // Store the original DataTable
                dataGridView1.DataSource = dt;
                PopulateColumnListComboBox();
                PopulateCategoryComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading XML file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateCategoryComboBox()
        {
            comboBoxCategories?.Items.Clear();
            if (originalDataTable != null)
            {
                var categories = originalDataTable.AsEnumerable()
                                    .Select(row => row["Category"]?.ToString())
                                    .Where(category => !string.IsNullOrEmpty(category))
                                    .Distinct()
                                    .ToList();
                comboBoxCategories?.Items.AddRange(categories.ToArray());
            }
        }

        private void PopulateColumnListComboBox()
        {
            if (comboBoxColumns == null)
                return;

            comboBoxColumns.Items.Clear();

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                comboBoxColumns.Items.Add(column.HeaderText);
            }

            if (comboBoxColumns.Items.Count > 0)
            {
                comboBoxColumns.SelectedIndex = 0;
            }
        }

        private void comboBoxCategories_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string? selectedCategory = comboBoxCategories?.SelectedItem?.ToString();
            DataTable? dataTable = dataGridView1.DataSource as DataTable;

            if (selectedCategory != null && dataTable != null)
            {
                dataTable.DefaultView.RowFilter = selectedCategory == "All" ? string.Empty : $"Category = '{selectedCategory}'";
            }
        }

        private void PopulateContentList()
        {
            // Clear previous items
            listBoxContent!.Items.Clear();

            // Get the selected column name
            string? selectedColumnName = comboBoxColumns?.SelectedItem?.ToString();

            // Check if selectedColumnName is null before proceeding
            if (selectedColumnName != null && originalDataTable != null)
            {
                // Use HashSet to ensure uniqueness
                var uniqueValues = new HashSet<string>();

                foreach (DataRow row in originalDataTable.Rows)
                {
                    string? value = row[selectedColumnName]?.ToString();
                    if (!string.IsNullOrEmpty(value) && uniqueValues.Add(value))
                    {
                        listBoxContent.Items.Add(value);
                    }
                }
            }
        }



        // private void listBoxContent_SelectedIndexChanged(object? sender, EventArgs e)
        // {
        //     if (listBoxContent.SelectedItem != null)
        //     {
        //         string selectedContent = listBoxContent.SelectedItem.ToString();
        //         string selectedColumn = comboBoxColumns.SelectedItem.ToString();

        //         // Get selected rows
        //         foreach (DataGridViewRow row in dataGridView1.SelectedRows)
        //         {
        //             // Check if the row is not a new row
        //             if (!row.IsNewRow)
        //             {
        //                 // Find the index of the selected column
        //                 int columnIndex = dataGridView1.Columns[selectedColumn].Index;

        //                 // Update the cell value with the selected content
        //                 row.Cells[columnIndex].Value = selectedContent;
        //             }
        //         }
        //     }
        // }

        private void listBoxContent_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listBoxContent.SelectedItem != null)
            {
                string selectedContent = listBoxContent.SelectedItem.ToString();
                string selectedColumn = comboBoxColumns.SelectedItem.ToString();

                // Get selected rows
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    // Check if the row is not a new row
                    if (!row.IsNewRow)
                    {
                        // Find the index of the selected column
                        int columnIndex = dataGridView1.Columns[selectedColumn].Index;

                        // Update the cell value with the selected content
                        row.Cells[columnIndex].Value = selectedContent;
                    }
                }
            }
        }

       // Method to encode special characters for XML
        private string EncodeSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&apos;");
        }



    }
}
