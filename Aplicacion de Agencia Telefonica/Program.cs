using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SistemaAgendaTelefonica
{
    // =========================================================================
    // MODELO DE DATOS - CLASE CONTACTO
    // =========================================================================
    /// <summary>
    /// Entidad que representa la estructura de datos de un Contacto en la Agenda.
    /// </summary>
    public class Contacto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }

    // =========================================================================
    // REPOSITORIO Y LÓGICA DE NEGOCIO (CRUD)
    // =========================================================================
    /// <summary>
    /// Maneja el almacenamiento en memoria y las operaciones CRUD de la Agenda.
    /// </summary>
    public class AgendaRepository
    {
        private readonly List<Contacto> _contactos = new List<Contacto>();
        private int _nextId = 1;

        public AgendaRepository()
        {
            // Contactos predeterminados de prueba (Datos de Semilla)
            Agregar(new Contacto { Nombre = "Guillermo", Apellido = "Pérez Mancía", Telefono = "7890-1234", Correo = "gperez@usam.edu.sv", Categoria = "Docente" });
            Agregar(new Contacto { Nombre = "Carlos", Apellido = "Martínez", Telefono = "7123-4567", Correo = "carlos.m@estudiantec.edu.sv", Categoria = "USAM" });
        }

        public bool Agregar(Contacto nuevo)
        {
            if (nuevo == null) return false;
            nuevo.Id = _nextId++;
            _contactos.Add(nuevo);
            return true;
        }

        public List<Contacto> ObtenerTodos() => new List<Contacto>(_contactos);

        public List<Contacto> Buscar(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio)) return ObtenerTodos();
            return _contactos.Where(c =>
                c.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase) ||
                c.Apellido.Contains(criterio, StringComparison.OrdinalIgnoreCase) ||
                c.Telefono.Contains(criterio, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public bool Actualizar(Contacto modificado)
        {
            var existente = _contactos.FirstOrDefault(c => c.Id == modificado.Id);
            if (existente == null) return false;

            existente.Nombre = modificado.Nombre;
            existente.Apellido = modificado.Apellido;
            existente.Telefono = modificado.Telefono;
            existente.Correo = modificado.Correo;
            existente.Categoria = modificado.Categoria;
            return true;
        }

        public bool Eliminar(int id)
        {
            var item = _contactos.FirstOrDefault(c => c.Id == id);
            return item != null && _contactos.Remove(item);
        }
    }

    // =========================================================================
    // FORMULARIO PRINCIPAL (INTERFAZ GRÁFICA DE USUARIO WINFORMS)
    // =========================================================================
    /// <summary>
    /// Formulario interactivo que cumple con las buenas prácticas de UI y C# de la USAM.
    /// </summary>
    public class Form1 : Form
    {
        private readonly AgendaRepository _repo = new AgendaRepository();
        private int _selectedId = -1; // Almacena el ID del contacto seleccionado en la tabla

        // Componentes UI de la aplicación
        private Label lblTitulo = null!;
        private Label lblNombre = null!, lblApellido = null!, lblTelefono = null!, lblCorreo = null!, lblCategoria = null!;
        private TextBox txtNombre = null!, txtApellido = null!, txtTelefono = null!, txtCorreo = null!, txtBuscar = null!;
        private ComboBox cbxCategoria = null!;
        private Button btnAgregar = null!, btnActualizar = null!, btnEliminar = null!, btnLimpiar = null!;
        private DataGridView dgvContactos = null!;
        private Panel panelHeader = null!, panelForm = null!;

        public Form1()
        {
            InitializeComponentCustom();
            CargarDatosTabla();
        }

        /// <summary>
        /// Construcción visual por código del Formulario (compatible sin requerir archivo .Designer.cs extra).
        /// </summary>
        private void InitializeComponentCustom()
        {
            this.Text = "Sistema de Agenda Telefónica ";
            this.Size = new Size(950, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Panel Superior (Encabezado Institucional)
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(15, 76, 129) // Azul institucional USAM
            };

            lblTitulo = new Label
            {
                Text = "UNIVERSIDAD SALVADOREÑA ALBERTO MASFERRER\nSistema de Agenda Telefónica - Programación I",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelHeader.Controls.Add(lblTitulo);

            // Panel Izquierdo para Formulario de Registro
            panelForm = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(320, 470),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Controles de Entrada (Labels & TextBoxes)
            lblNombre = CrearLabel("Nombre:", 15, 20);
            txtNombre = CrearTextBox(15, 42);

            lblApellido = CrearLabel("Apellido:", 15, 90);
            txtApellido = CrearTextBox(15, 112);

            lblTelefono = CrearLabel("Teléfono:", 15, 160);
            txtTelefono = CrearTextBox(15, 182);

            lblCorreo = CrearLabel("Correo Electrónico:", 15, 230);
            txtCorreo = CrearTextBox(15, 252);

            lblCategoria = CrearLabel("Categoría:", 15, 300);
            cbxCategoria = new ComboBox
            {
                Location = new Point(15, 322),
                Size = new Size(285, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cbxCategoria.Items.AddRange(new string[] { "Personal", "Trabajo", "USAM", "Familia", "Otros" });
            cbxCategoria.SelectedIndex = 0;

            // Botones del Formulario
            btnAgregar = CrearBoton("Guardar", Color.FromArgb(40, 167, 69), 15, 370, 135);
            btnActualizar = CrearBoton("Modificar", Color.FromArgb(255, 193, 7), 165, 370, 135);
            btnEliminar = CrearBoton("Eliminar", Color.FromArgb(220, 53, 69), 15, 415, 135);
            btnLimpiar = CrearBoton("Limpiar", Color.FromArgb(108, 117, 125), 165, 415, 135);

            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;

            // Registro de Eventos de Botones
            btnAgregar.Click += BtnAgregar_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += (s, e) => LimpiarFormulario();

            panelForm.Controls.AddRange(new Control[] {
                lblNombre, txtNombre, lblApellido, txtApellido,
                lblTelefono, txtTelefono, lblCorreo, txtCorreo,
                lblCategoria, cbxCategoria, btnAgregar, btnActualizar, btnEliminar, btnLimpiar
            });

            // Buscador
            Label lblBuscar = new Label { Text = "Buscar Contacto:", Location = new Point(360, 93), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            txtBuscar = new TextBox { Location = new Point(480, 90), Size = new Size(430, 27), Font = new Font("Segoe UI", 9.5f) };
            txtBuscar.TextChanged += (s, e) => CargarDatosTabla(txtBuscar.Text);

            // DataGridView (Tabla de contactos)
            dgvContactos = new DataGridView
            {
                Location = new Point(360, 130),
                Size = new Size(550, 430),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };
            dgvContactos.SelectionChanged += DgvContactos_SelectionChanged;

            // Agregar Controles al Formulario
            this.Controls.Add(panelHeader);
            this.Controls.Add(panelForm);
            this.Controls.Add(lblBuscar);
            this.Controls.Add(txtBuscar);
            this.Controls.Add(dgvContactos);
        }

        #region Helpers de UI
        private Label CrearLabel(string texto, int x, int y)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41)
            };
        }

        private TextBox CrearTextBox(int x, int y)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(285, 27),
                Font = new Font("Segoe UI", 9.5f)
            };
        }

        private Button CrearBoton(string texto, Color color, int x, int y, int ancho)
        {
            return new Button
            {
                Text = texto,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(x, y),
                Size = new Size(ancho, 35),
                Cursor = Cursors.Hand
            };
        }
        #endregion

        #region Manejadores de Eventos (Event Handlers)

        private void CargarDatosTabla(string filtro = "")
        {
            dgvContactos.DataSource = null;
            dgvContactos.DataSource = _repo.Buscar(filtro);

            if (dgvContactos.Columns["Id"] != null)
                dgvContactos.Columns["Id"].Width = 40;
        }

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos()) return;

                var nuevo = new Contacto
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Correo = txtCorreo.Text.Trim(),
                    Categoria = cbxCategoria.SelectedItem?.ToString() ?? "General"
                };

                if (_repo.Agregar(nuevo))
                {
                    MessageBox.Show("¡Contacto guardado exitosamente!", "Éxito - USAM Agenda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatosTabla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar: {ex.Message}", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnActualizar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedId == -1) return;
                if (!ValidarCampos()) return;

                var modificado = new Contacto
                {
                    Id = _selectedId,
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Correo = txtCorreo.Text.Trim(),
                    Categoria = cbxCategoria.SelectedItem?.ToString() ?? "General"
                };

                if (_repo.Actualizar(modificado))
                {
                    MessageBox.Show("Contacto modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatosTabla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedId == -1) return;

                DialogResult dr = MessageBox.Show("¿Está seguro que desea eliminar este contacto?", "Confirmación de Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    if (_repo.Eliminar(_selectedId))
                    {
                        MessageBox.Show("Contacto eliminado correctamente.", "Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDatosTabla();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvContactos_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvContactos.SelectedRows.Count > 0)
            {
                var row = dgvContactos.SelectedRows[0];
                var contacto = (Contacto)row.DataBoundItem;

                _selectedId = contacto.Id;
                txtNombre.Text = contacto.Nombre;
                txtApellido.Text = contacto.Apellido;
                txtTelefono.Text = contacto.Telefono;
                txtCorreo.Text = contacto.Correo;
                cbxCategoria.SelectedItem = contacto.Categoria;

                btnAgregar.Enabled = false;
                btnActualizar.Enabled = true;
                btnEliminar.Enabled = true;
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo 'Nombre' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("El campo 'Teléfono' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return false;
            }
            return true;
        }

        private void LimpiarFormulario()
        {
            _selectedId = -1;
            txtNombre.Clear();
            txtApellido.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            if (cbxCategoria.Items.Count > 0) cbxCategoria.SelectedIndex = 0;

            btnAgregar.Enabled = true;
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;
            dgvContactos.ClearSelection();
        }

        #endregion

        /// <summary>
        /// Punto de entrada del Formulario WinForms.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}