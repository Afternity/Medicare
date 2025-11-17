using Medicare.ClientWPF.Profile;
using Medicare.ClientWPF.Windows;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Medicare.Tests.Tests.WindowTests.MainWindowTests
{
    public class MainWindowCreateTests
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

        private async Task<T> InvokePrivateMethodAsync<T>(MainWindow window, string methodName, params object[] parameters)
        {
            var method = typeof(MainWindow).GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                throw new ArgumentException($"Method {methodName} not found");

            var task = method.Invoke(window, parameters) as Task<T>;
            return await task;
        }

        [StaFact]
        public async Task GetAllAsync_WithAppointments_ReturnsAllAppointments()
        {
            // Arrange
            _context = CreateInMemoryContext();

            // Создаем тестового доктора и добавляем в базу
            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            // Создаем тестовых пациентов
            var patient1 = new Patient { Id = Guid.NewGuid(), FullName = "Пациент 1", Phone = "+79123456789" };
            var patient2 = new Patient { Id = Guid.NewGuid(), FullName = "Пациент 2", Phone = "+79123456780" };

            await _context.Patients.AddRangeAsync(patient1, patient2);
            await _context.SaveChangesAsync();

            // Создаем тестовые записи
            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Now.AddDays(1),
                    Status = "Scheduled",
                    Notes = "Первичный осмотр",
                    PatientId = patient1.Id,
                    DoctorId = doctor.Id
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Now.AddDays(2),
                    Status = "Completed",
                    Notes = "Повторный осмотр",
                    PatientId = patient2.Id,
                    DoctorId = doctor.Id
                }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            var window = new MainWindow();

            // Используем рефлексию для установки контекста
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            // Act
            var result = await InvokePrivateMethodAsync<IList<Appointment>>(window, "GetAllAsync");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [StaFact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
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

            // Act
            var result = await InvokePrivateMethodAsync<IList<Appointment>>(window, "GetAllAsync");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [StaFact]
        public async Task GetAllAsync_AppointmentsWithRelatedData_LoadsNavigationProperties()
        {
            // Arrange
            _context = CreateInMemoryContext();

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var patient = new Patient { Id = Guid.NewGuid(), FullName = "Тестовый пациент", Phone = "+79123456789" };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                AppointmentDate = DateTime.Now.AddDays(1),
                Status = "Scheduled",
                Notes = "Тестовый осмотр",
                PatientId = patient.Id,
                DoctorId = doctor.Id
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            // Act
            var result = await InvokePrivateMethodAsync<IList<Appointment>>(window, "GetAllAsync");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var loadedAppointment = result.First();
            Assert.NotNull(loadedAppointment.Patient);
            Assert.Equal("Тестовый пациент", loadedAppointment.Patient.FullName);
            Assert.Equal("+79123456789", loadedAppointment.Patient.Phone);
        }

        public void Dispose()
        {
            _context?.Dispose();
            DoctorProfile.Profile = null;
        }
    }
}
