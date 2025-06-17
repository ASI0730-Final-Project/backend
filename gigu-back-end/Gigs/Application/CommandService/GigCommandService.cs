using Gigs.Domain.Models.Commands;
using Gigs.Domain.Models.Entities;
using Gigs.Domain.Models.Exceptions;
using Gigs.Domain.Models.Validators;
using Gigs.Domain;
using Gigs.Domain.Services;
using FluentValidation;
using System.Threading.Tasks;

namespace Gigs.Application.CommandService
{
    public class GigCommandService
    {
        private readonly IGigRepository _gigRepository;
        private readonly IGigDomainService _gigDomainService;
        private readonly CreateGigCommandValidator _createValidator;
        private readonly UpdateGigCommandValidator _updateValidator;

        public GigCommandService(
            IGigRepository gigRepository,
            IGigDomainService gigDomainService,
            CreateGigCommandValidator createValidator,
            UpdateGigCommandValidator updateValidator)
        {
            _gigRepository = gigRepository;
            _gigDomainService = gigDomainService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Gig> CreateGigAsync(CreateGigCommand command)
        {
            // Validar comando
            var validationResult = await _createValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new GigValidationException($"Validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            // Validaciones de dominio
            if (!await _gigDomainService.IsUserActiveFreelancerAsync(command.UserId))
            {
                throw new GigValidationException("User is not an active freelancer");
            }

            if (!await _gigDomainService.IsCategoryValidAsync(command.Category))
            {
                throw new GigValidationException($"Category '{command.Category}' is not valid");
            }

            // Crear entidad
            var gig = new Gig(
                command.Title,
                command.Description,
                command.Price,
                command.UserId,
                command.Category,
                command.DeliveryDays
            );

            // Guardar en repositorio
            return await _gigRepository.CreateAsync(gig);
        }

        public async Task<Gig> UpdateGigAsync(UpdateGigCommand command)
        {
            // Validar comando
            var validationResult = await _updateValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new GigValidationException($"Validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            // Verificar que el gig existe
            var existingGig = await _gigRepository.GetByIdAsync(command.Id);
            if (existingGig == null)
            {
                throw new GigNotFoundException(command.Id);
            }

            // Validar categoría
            if (!await _gigDomainService.IsCategoryValidAsync(command.Category))
            {
                throw new GigValidationException($"Category '{command.Category}' is not valid");
            }

            // Actualizar propiedades
            existingGig.Title = command.Title;
            existingGig.Description = command.Description;
            existingGig.Price = command.Price;
            existingGig.Category = command.Category;
            existingGig.DeliveryDays = command.DeliveryDays;

            // Guardar cambios
            return await _gigRepository.UpdateAsync(existingGig);
        }

        public async Task<bool> DeleteGigAsync(int gigId, int userId)
        {
            // Verificar propiedad del gig
            if (!await _gigDomainService.ValidateGigOwnershipAsync(gigId, userId))
            {
                throw new GigValidationException("User is not authorized to delete this gig");
            }

            return await _gigRepository.DeleteAsync(gigId);
        }
    }
}