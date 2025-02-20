﻿using SistemaFacturacion.CLASES;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace SistemaFacturacion.USUARIOS
{
    public partial class Login : Window
    {
        // Instancia de la clase Autenticacion
        private Autenticacion autenticacion;

        public Login()
        {
            InitializeComponent();
            autenticacion = new Autenticacion(); // Crear la instancia de autenticación
        }

        // Permite mover la ventana.
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // Minimiza la ventana.
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Cierra la aplicación.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Maneja el evento de inicio de sesión.
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPass.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingresa todos los campos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar las credenciales usando la clase Autenticacion
            if (ValidarUsuario(username, password))
            {
                // Si la validación es exitosa, asignar el usuario autenticado
                if (autenticacion.IniciarSesion(username, password))
                {
                    MessageBox.Show($"Bienvenido, {username}!", "Inicio de Sesión Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Abrir la ventana principal.
                    this.Hide();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método para validar credenciales desde la base de datos.
        private bool ValidarUsuario(string username, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FacturacionDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta para validar el usuario.
                    string query = "SELECT Contraseña FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Activo = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreUsuario", username);

                        var passwordFromDb = cmd.ExecuteScalar() as string;

                        if (!string.IsNullOrEmpty(passwordFromDb))
                        {
                            // Comparar la contraseña en texto claro (sin hash).
                            return password == passwordFromDb;
                        }
                        else
                        {
                            MessageBox.Show("El usuario no está activo o no existe.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Error de conexión con la base de datos: {ex.Message}", "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return false;
        }

        // Restablecimiento de la contraseña.
        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para restablecer la contraseña si es necesario.
        }

        private void btnCrearUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Crear una nueva instancia de la ventana CrearUsuario
            CrearUsuario crearUsuarioWindow = new CrearUsuario();
            crearUsuarioWindow.Show(); // Mostrar la ventana de registro
            this.Hide(); // Ocultar la ventana de login
        }
    }
}
