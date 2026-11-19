using Application.Interfaces;
using Application.Models.Shift;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IPetRepository _petRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;

        public ShiftService(IShiftRepository shiftRepository, IPetRepository petRepository, IUserRepository userRepository, IServiceRepository serviceRepository)
        {
            _shiftRepository = shiftRepository;
            _petRepository = petRepository;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<List<ShiftDTO>> GetAllAsync()
        {
            var shifts = await _shiftRepository.GetAllAsync();
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<ShiftDTO?> GetByIdAsync(int id)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            if (shift == null) return null;

            return MapDTO(shift);
        }

        public async Task<List<ShiftDTO>> GetByClientIdAsync(int clientId)
        {
            var shifts = await _shiftRepository.GetByClientIdAsync(clientId);
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<List<ShiftDTO>> GetByVeterinaryIdAsync(int veterinaryId)
        {
            var shifts = await _shiftRepository.GetByVeterinaryIdAsync(veterinaryId);
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<List<ShiftDTO>> GetByPetIdAsync(int petId)
        {
            var shifts = await _shiftRepository.GetByPetIdAsync(petId);
            return shifts.Select(MapDTO).ToList();
        }

        public async Task<List<ShiftDTO>> GetByStatusAsync(ShiftStatus status)
        {
            var shifts = await _shiftRepository.GetByStatusAsync(status);
            return shifts.Select(MapDTO).ToList();
        }


        public async Task<ShiftDTO?> AddAsync(CreateShiftDTO shift) 
        {
            if (shift == null) return null;

            var pet = await _petRepository.GetByIdAsync(shift.PetId)
                ?? throw new NotFoundException($"La mascota con id {shift.PetId} no existe");

            var client = await _userRepository.GetByIdAsync(shift.ClientId)
                ?? throw new NotFoundException($"El cliente con id {shift.ClientId} no existe");

            var vet = await _userRepository.GetByIdAsync(shift.VeterinaryId)
                ?? throw new NotFoundException($"El veterinario con id {shift.VeterinaryId} no existe");

            var service = await _serviceRepository.GetByIdAsync(shift.ServiceId)
                ?? throw new NotFoundException($"El servicio con id {shift.ServiceId} no existe");

            if (pet.ClientId != client.Id)
                throw new ConflictException("La mascota no pertenece a ese cliente");

            if (vet.Rol != Role.Veterinario)
                throw new ConflictException("El usuario no es un veterinario");

            var newShift = new Shift
            {
                PetId = shift.PetId,
                VeterinaryId = shift.VeterinaryId,
                ClientId = shift.ClientId,
                ServiceId = shift.ServiceId,
                Status = ShiftStatus.Pendiente,
                ClientObs = shift.ClientObs,
                InitialDate = shift.InitialDate,
                EndDate = null
            };

            var addedShift = await _shiftRepository.AddAsync(newShift);
            if (addedShift == null || addedShift.Id == 0) return null;
            await _shiftRepository.SaveChangesAsync();

            return MapDTO(addedShift);
        }

        public async Task<ShiftDTO?> UpdateAsync(ShiftDTO shift)
        {
            if (shift == null) return null;

            var updateShift = await _shiftRepository.GetByIdAsync(shift.Id);
            if (updateShift == null) return null;

            updateShift.PetId = shift.PetId;
            updateShift.VeterinaryId = shift.VeterinaryId;
            updateShift.ClientId = shift.ClientId;
            updateShift.ServiceId = shift.ServiceId;
            updateShift.Status = shift.Status;
            updateShift.ClientObs = shift.ClientObs;
            updateShift.InitialDate = shift.InitialDate;
            updateShift.EndDate = shift.EndDate;

            await _shiftRepository.UpdateAsync(updateShift);
            await _shiftRepository.SaveChangesAsync();

            return MapDTO(updateShift);
        }

        public async Task<ShiftDTO?> DeleteAsync(int id)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            if (shift == null) return null;

            await _shiftRepository.DeleteAsync(shift.Id);
            await _shiftRepository.SaveChangesAsync();
            return MapDTO(shift);
        }

        private static ShiftDTO MapDTO(Shift s)
        {
            return new ShiftDTO
            {
                Id = s.Id,
                PetId = s.PetId,
                VeterinaryId = s.VeterinaryId,
                ClientId = s.ClientId,
                ServiceId = s.ServiceId,
                Status = s.Status,
                ClientObs = s.ClientObs,
                InitialDate = s.InitialDate,
                EndDate = s.EndDate
            };
        }
    }
}