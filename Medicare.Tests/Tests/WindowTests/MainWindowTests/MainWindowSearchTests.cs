using Medicare.ClientWPF.DTOs;
using Medicare.ClientWPF.Profile;
using Medicare.ClientWPF.Windows;
using Medicare.Domain.Data.DbContexts;
using Medicare.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Medicare.Tests.Tests.WindowTests.MainWindowTests
{
    public class MainWindowSearchTests
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
        public async Task DeepSearch_NoFilters_ReturnsAllAppointments()
        {
            // Arrange
            _context = CreateInMemoryContext();

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var patient1 = new Patient { Id = Guid.NewGuid(), FullName = "Пациент 1", Phone = "+79123456789" };
            var patient2 = new Patient { Id = Guid.NewGuid(), FullName = "Пациент 2", Phone = "+79123456780" };

            await _context.Patients.AddRangeAsync(patient1, patient2);
            await _context.SaveChangesAsync();

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Now.AddDays(1),
                    Status = "Scheduled",
                    Notes = "Осмотр 1",
                    PatientId = patient1.Id,
                    DoctorId = doctor.Id
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Now.AddDays(2),
                    Status = "Completed",
                    Notes = "Осмотр 2",
                    PatientId = patient2.Id,
                    DoctorId = doctor.Id
                }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            var searchDto = new AppointmentDto
            {
                PatientFullName = "",
                PatientPhone = "",
                AppointmentNote = "",
                AppointmentDate = DateTime.MinValue
            };

            // Act
            var result = await InvokePrivateMethodAsync<IList<Appointment>>(window, "DeepSearch", searchDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [StaFact]
        public async Task DeepSearch_ByPatientFullName_ReturnsFilteredAppointments()
        {
            // Arrange
            _context = CreateInMemoryContext();

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var patient1 = new Patient { Id = Guid.NewGuid(), FullName = "Иван Иванов", Phone = "+79123456789" };
            var patient2 = new Patient { Id = Guid.NewGuid(), FullName = "Петр Петров", Phone = "+79123456780" };

            await _context.Patients.AddRangeAsync(patient1, patient2);
            await _context.SaveChangesAsync();

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Now.AddDays(1),
                    Status = "Scheduled",
                    Notes = "Осмотр",
                    PatientId = patient1.Id,
                    DoctorId = doctor.Id
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Now.AddDays(2),
                    Status = "Completed",
                    Notes = "Осмотр",
                    PatientId = patient2.Id,
                    DoctorId = doctor.Id
                }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            var searchDto = new AppointmentDto
            {
                PatientFullName = "Иван",
                PatientPhone = "",
                AppointmentNote = "",
                AppointmentDate = DateTime.MinValue
            };

            // Act
            var result = await InvokePrivateMethodAsync<IList<Appointment>>(window, "DeepSearch", searchDto);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Иван Иванов", result.First().Patient.FullName);
        }

        [StaFact]
        public async Task DeepSearch_ByAppointmentDate_ReturnsFilteredAppointments()
        {
            // Arrange
            _context = CreateInMemoryContext();

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var patient = new Patient { Id = Guid.NewGuid(), FullName = "Пациент", Phone = "+79123456789" };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var targetDate = DateTime.Now.AddDays(1).Date;

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = targetDate,
                    Status = "Scheduled",
                    Notes = "Осмотр сегодня",
                    PatientId = patient.Id,
                    DoctorId = doctor.Id
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Now.AddDays(5).Date,
                    Status = "Scheduled",
                    Notes = "Осмотр позже",
                    PatientId = patient.Id,
                    DoctorId = doctor.Id
                }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            var searchDto = new AppointmentDto
            {
                PatientFullName = "",
                PatientPhone = "",
                AppointmentNote = "",
                AppointmentDate = targetDate
            };

            // Act
            var result = await InvokePrivateMethodAsync<IList<Appointment>>(window, "DeepSearch", searchDto);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(targetDate, result.First().AppointmentDate.Date);
        }

        [StaFact]
        public async Task DeepSearch_NoResults_ReturnsEmptyList()
        {
            // Arrange
            _context = CreateInMemoryContext();

            var doctor = new Doctor { Id = Guid.NewGuid(), FullName = "Тестовый доктор" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            DoctorProfile.Profile = doctor;

            var patient = new Patient { Id = Guid.NewGuid(), FullName = "Пациент", Phone = "+79123456789" };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                AppointmentDate = DateTime.Now.AddDays(1),
                Status = "Scheduled",
                Notes = "Осмотр",
                PatientId = patient.Id,
                DoctorId = doctor.Id
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var window = new MainWindow();
            var contextField = typeof(MainWindow).GetField("_context",
                BindingFlags.NonPublic | BindingFlags.Instance);
            contextField.SetValue(window, _context);

            var searchDto = new AppointmentDto
            {
                PatientFullName = "Несуществующий",
                PatientPhone = "",
                AppointmentNote = "",
                AppointmentDate = DateTime.MinValue
            };

            // Act
            var result = await InvokePrivateMethodAsync<IList<Appointment>>(window, "DeepSearch", searchDto);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
            DoctorProfile.Profile = null;
        }
    }
}
