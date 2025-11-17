using Medicare.ClientWPF.Profile;
using Medicare.ClientWPF.Windows;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Medicare.Tests.Tests.WindowTests.MainWindowTests
{
    public class MainWindowPatientProfileTests
        : IDisposable
    {
        private MedicareDbContext _context;

        private MedicareDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MedicareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new MedicareDbContext(options);
        }

        private async Task InvokePrivateMethodAsync(MainWindow window, string methodName, params object[] parameters)
        {
            var method = typeof(MainWindow).GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                throw new ArgumentException($"Method {methodName} not found");

            var task = method.Invoke(window, parameters) as Task;
            await task;
        }

        [StaFact]
        public async Task GetPatientProfile_ValidPatientId_LoadsPatientProfile()
        {
            // Arrange
            _context = CreateInMemoryContext();

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "Тестовый пациент",
                Phone = "+79123456789",
                Email = "test@example.com"
            };

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            // Очищаем профиль перед тестом
            PatientProfile.Profile = null;

            // Act
            await InvokePrivateMethodAsync(window, "GetPatientProfile", patient.Id);

            // Assert
            Assert.NotNull(PatientProfile.Profile);
            Assert.Equal("Тестовый пациент", PatientProfile.Profile.FullName);
            Assert.Equal("+79123456789", PatientProfile.Profile.Phone);
        }

        [StaFact]
        public async Task GetPatientProfile_NonExistentPatientId_ShowsWarning()
        {
            // Arrange
            _context = CreateInMemoryContext();

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            var nonExistentId = Guid.NewGuid();

            // Сохраняем текущий профиль
            var originalProfile = PatientProfile.Profile;

            // Act
            await InvokePrivateMethodAsync(window, "GetPatientProfile", nonExistentId);

            // Assert - Profile не должен измениться
            Assert.Equal(originalProfile, PatientProfile.Profile);
        }

        [StaFact]
        public async Task GetPatientProfile_EmptyDatabase_ShowsWarning()
        {
            // Arrange
            _context = CreateInMemoryContext(); // Пустая база

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            var testId = Guid.NewGuid();
            var originalProfile = PatientProfile.Profile;

            // Act
            await InvokePrivateMethodAsync(window, "GetPatientProfile", testId);

            // Assert - Profile не должен измениться
            Assert.Equal(originalProfile, PatientProfile.Profile);
        }

        public void Dispose()
        {
            _context?.Dispose();
            PatientProfile.Profile = null;
            DoctorProfile.Profile = null;
        }
    }
}
