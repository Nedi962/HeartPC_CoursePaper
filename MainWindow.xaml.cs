using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Management;
using System.Windows.Threading;
//using OpenHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware;
using LiveCharts;
using LiveCharts.Wpf;
using System.IO;

namespace HeartPC
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Computer computer;
        private DispatcherTimer timer;
        public ChartValues<double> TemperatureValues { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            LoadSystemInfo();
            LoadSensorData();

            TemperatureValues = new ChartValues<double>();
            DataContext = this;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
    }

        private void LoadSystemInfo()
        {
            // CPU Information
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                string cpuName = obj["Name"].ToString();
                string cpuManufacturer = obj["Manufacturer"].ToString();
                string cpuDescription = obj["Description"].ToString();
                string cpuId = obj["ProcessorId"].ToString();

                cpuInfoLabel.Text = $"CPU: {cpuName}\nManufacturer: {cpuManufacturer}\nDescription: {cpuDescription}\nProcessorId: {cpuId}";
            }

            // RAM Information
            searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher.Get())
            {
                ulong capacity = Convert.ToUInt64(obj["Capacity"]);
                string ramManufacturer = obj["Manufacturer"].ToString();
                string ramPartNumber = obj["PartNumber"].ToString();

                ramInfoLabel.Text = $"RAM: {capacity / (1024 * 1024)} MB\nManufacturer: {ramManufacturer}\nPartNumber: {ramPartNumber}";
            }

            // Motherboard Information
            searcher = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            foreach (ManagementObject obj in searcher.Get())
            {
                string manufacturer = obj["Manufacturer"].ToString();
                string product = obj["Product"].ToString();
                string version = obj["Version"].ToString();
                string serialNumber = obj["SerialNumber"].ToString();

                motherboardInfoLabel.Text = $"Manufacturer: {manufacturer}\nProduct: {product}\nVersion: {version}\nSerial Number: {serialNumber}";
            }

            // Hard Drive Information
            searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            foreach (ManagementObject obj in searcher.Get())
            {
                string model = obj["Model"].ToString();
                string interfaceType = obj["InterfaceType"].ToString();
                ulong size = Convert.ToUInt64(obj["Size"]);

                hardDriveInfoLabel.Text = $"Model: {model}\nInterface Type: {interfaceType}\nSize: {size / (1024 * 1024 * 1024)} GB";
            }

            // GPU Information
            searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                string gpuName = obj["Name"].ToString();
                string gpuDeviceID = obj["DeviceID"].ToString();
                string gpuDriverVersion = obj["DriverVersion"].ToString();

                gpuInfoLabel.Text = $"GPU: {gpuName}\nDevice ID: {gpuDeviceID}\nDriver Version: {gpuDriverVersion}";
            }

            // System Information
            searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                string osCaption = obj["Caption"].ToString();
                string osVersion = obj["Version"].ToString();
                string osManufacturer = obj["Manufacturer"].ToString();
                string osInstallDate = obj["InstallDate"].ToString();

                systemInfoLabel.Text = $"OS: {osCaption}\nVersion: {osVersion}\nManufacturer: {osManufacturer}\nInstall Date: {osInstallDate}";
            }
        }

        private void LoadSensorData()
        {
            computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsStorageEnabled = true
            };

            try
            {
                computer.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening computer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var hardware in computer.Hardware)
            {
                hardware.Update();
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("CPU"))
                    {
                        cpuTempLabel.Text = $"CPU Temperature: {sensor.Value} °C";
                        TemperatureValues.Add((double)sensor.Value);
                    }
                    else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU"))
                    {
                        // CPU Load
                    }
                    else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("CPU"))
                    {
                        // CPU Power
                    }
                    else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU"))
                    {
                        // CPU Clock
                    }
                    else if (sensor.SensorType == SensorType.Voltage && sensor.Name.Contains("CPU"))
                    {
                        // CPU Voltage
                    }
                    else if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("GPU"))
                    {
                        // GPU Temperature
                    }
                    else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("GPU"))
                    {
                        // GPU Load
                    }
                    else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("GPU"))
                    {
                        // GPU Power
                    }
                    else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("GPU"))
                    {
                        // GPU Clock
                    }
                    else if (sensor.SensorType == SensorType.Voltage && sensor.Name.Contains("GPU"))
                    {
                        // GPU Voltage
                    }
                    else if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Motherboard"))
                    {
                        // Motherboard Temperature
                    }
                    else if (sensor.SensorType == SensorType.Voltage && sensor.Name.Contains("Motherboard"))
                    {
                        // Motherboard Voltage
                    }
                    else if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Storage"))
                    {
                        // Storage Temperature
                    }
                    else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Memory"))
                    {
                        // Memory Load
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            LoadSensorData();
        }

        private void RefreshCpuInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemInfo();
        }

        private void RefreshRamInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemInfo();
        }

        private void RefreshCpuTemp_Click(object sender, RoutedEventArgs e)
        {
            LoadSensorData();
        }

        private void RefreshMotherboardInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemInfo();
        }

        private void RefreshHardDriveInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemInfo();
        }

        private void RefreshGpuInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemInfo();
        }

        private void RefreshSystemInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemInfo();
        }

        private void RefreshComponentsInfo_Click(object sender, RoutedEventArgs e)
        {
            LoadComponentsInfo();
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings menu is not implemented yet.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help menu is not implemented yet.", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RunCpuTest_Click(object sender, RoutedEventArgs e)
        {
            var testForm = new TestForm("CPU");
            testForm.ShowDialog();
        }

        private void RunRamTest_Click(object sender, RoutedEventArgs e)
        {
            var testForm = new TestForm("RAM");
            testForm.ShowDialog();
        }

        private void RunDiskTest_Click(object sender, RoutedEventArgs e)
        {
            var testForm = new TestForm("Disk");
            testForm.ShowDialog();
        }

        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (int i = 3; i * i <= number; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;
        }

        private void RunFurMarkTest_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string relativePath = System.IO.Path.Combine(projectDirectory, "HeartPC", "App", "FurMark_win64", "FurMark_GUI.exe");

            try
            {
                if (File.Exists(relativePath))
                {
                    // Запуск FurMark через командную строку
                    Process.Start(relativePath);
                }
                else
                {
                    MessageBox.Show($"File not found: {relativePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting FurMark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Run3DMarkTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Запуск 3DMark через командную строку
                Process.Start("C:\\Users\\lives\\OneDrive\\Рабочий стол\\College\\Курсовая Кукушкин\\3dmark\\3DMarkLauncher.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting 3DMark: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadComponentsInfo()
        {
            string componentsInfo = "";

            // CPU Information
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                string cpuName = obj["Name"].ToString();
                componentsInfo += $"CPU: {cpuName}\n";
            }

            // RAM Information
            searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher.Get())
            {
                ulong capacity = Convert.ToUInt64(obj["Capacity"]);
                string ramManufacturer = obj["Manufacturer"].ToString();
                componentsInfo += $"RAM: {capacity / (1024 * 1024)} MB, Manufacturer: {ramManufacturer}\n";
            }

            // Motherboard Information
            searcher = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            foreach (ManagementObject obj in searcher.Get())
            {
                string manufacturer = obj["Manufacturer"].ToString();
                string product = obj["Product"].ToString();
                string version = obj["Version"].ToString();
                string serialNumber = obj["SerialNumber"].ToString();
                componentsInfo += $"Motherboard: Manufacturer: {manufacturer}, Product: {product}, Version: {version}, Serial Number: {serialNumber}\n";
            }

            // Hard Drive Information
            searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            foreach (ManagementObject obj in searcher.Get())
            {
                string model = obj["Model"].ToString();
                ulong size = Convert.ToUInt64(obj["Size"]);
                componentsInfo += $"Hard Drive: Model: {model}, Size: {size / (1024 * 1024 * 1024)} GB\n";
            }

            // GPU Information
            searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                string gpuName = obj["Name"].ToString();
                componentsInfo += $"GPU: {gpuName}\n";
            }

            // System Information
            searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                string osCaption = obj["Caption"].ToString();
                string osVersion = obj["Version"].ToString();
                string osManufacturer = obj["Manufacturer"].ToString();
                string osInstallDate = obj["InstallDate"].ToString();

                componentsInfo += $"OS: {osCaption}, Version: {osVersion}, Manufacturer: {osManufacturer}, Install Date: {osInstallDate}\n";
            }

            componentsInfoLabel.Text = componentsInfo;
        }

    }
}

