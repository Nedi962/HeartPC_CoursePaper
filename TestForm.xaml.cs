using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HeartPC
{
    /// <summary>
    /// Логика взаимодействия для TestForm.xaml
    /// </summary>
    public partial class TestForm : Window
    {
        private CancellationTokenSource cancellationTokenSource;
        private int dataSize;
        private int cycles;
        private Stopwatch stopwatch;
        private string testType;

        public TestForm(string testType)
        {
            InitializeComponent();
            this.testType = testType;
        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(dataSizeTextBox.Text, out dataSize) && int.TryParse(cyclesTextBox.Text, out cycles))
            {
                ClearResults();
                cancellationTokenSource = new CancellationTokenSource();
                stopwatch = new Stopwatch();
                stopwatch.Start();
                Task.Run(() => RunTest(cancellationTokenSource.Token));
            }
            else
            {
                MessageBox.Show("Please enter valid data size and cycles.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopTest_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        private void RunTest(CancellationToken cancellationToken)
        {
            switch (testType)
            {
                case "CPU":
                    RunCpuTest(cancellationToken);
                    break;
                case "RAM":
                    RunRamTest(cancellationToken);
                    break;
                case "Disk":
                    RunDiskTest(cancellationToken);
                    break;
            }
        }

        private void RunCpuTest(CancellationToken cancellationToken)
        {
            int primeCount = 0;
            for (int cycle = 0; cycle < cycles; cycle++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                for (int i = 2; i < dataSize * 1000; i++) // Увеличим диапазон для более продолжительного теста
                {
                    if (IsPrime(i))
                    {
                        primeCount++;
                    }
                }

                UpdateProgress(cycle + 1, cycles);
            }

            stopwatch.Stop();
            UpdateResult($"CPU Test completed in {stopwatch.ElapsedMilliseconds} ms. Found {primeCount} prime numbers. Cycles: {cycles}, Data Size: {dataSize} MB.");
        }

        private void RunRamTest(CancellationToken cancellationToken)
        {
            int arraySize = dataSize * 1024 * 1024 / sizeof(int); // Convert MB to number of int elements
            int[] data = new int[arraySize];

            for (int cycle = 0; cycle < cycles; cycle++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                for (int i = 0; i < arraySize; i++)
                {
                    data[i] = i;
                }

                UpdateProgress(cycle + 1, cycles);
            }

            stopwatch.Stop();
            UpdateResult($"RAM Test completed in {stopwatch.ElapsedMilliseconds} ms. Cycles: {cycles}, Data Size: {dataSize} MB.");
        }

        private void RunDiskTest(CancellationToken cancellationToken)
        {
            string filePath = "testfile.tmp";
            int fileSize = dataSize * 1024 * 1024; // Convert MB to bytes
            byte[] data = new byte[fileSize];

            for (int cycle = 0; cycle < cycles; cycle++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // Убедимся, что файл не используется другим процессом
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(data, 0, data.Length);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fileStream.Read(data, 0, data.Length);
                }

                UpdateProgress(cycle + 1, cycles);
            }

            // Убедимся, что файл удален после использования
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            stopwatch.Stop();
            UpdateResult($"Disk Test completed in {stopwatch.ElapsedMilliseconds} ms. Cycles: {cycles}, Data Size: {dataSize} MB.");
        }

        private void UpdateProgress(int currentCycle, int totalCycles)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = (double)currentCycle / totalCycles * 100;
                progressTextBlock.Text = $"{progressBar.Value:0.00}%";
            });
        }

        private void UpdateResult(string result)
        {
            Dispatcher.Invoke(() =>
            {
                resultLabel.Text = result;
            });
        }

        private void ClearResults()
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = 0;
                progressTextBlock.Text = "0.00%";
                resultLabel.Text = string.Empty;
            });
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
    }
}

