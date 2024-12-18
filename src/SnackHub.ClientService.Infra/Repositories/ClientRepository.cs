using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SnackHub.ClientService.Domain.Contracts;
using SnackHub.ClientService.Domain.Entities;
using SnackHub.ClientService.Domain.ValueObjects;
using SnackHub.ClientService.Infra.Repositories.Context;

namespace SnackHub.ClientService.Infra.Repositories;

public class ClientRepository : BaseRepository<ClientModel, ClientDbContext>, IClientRepository
{
    public ClientRepository(ClientDbContext dbContext) : base(dbContext)
    {
    }

    public async Task AddAsync(ClientModel client)
    {
        await InsertAsync(client);
    }

    public async Task<ClientModel?> GetClientByIdAsync(Guid id)
    {
        return await FindByPredicateAsync(px => px.Id.Equals(id));
    }

    public async Task<ClientModel?> GetClientByCpfAsync(Cpf cpf)
    {
        return await FindByPredicateAsync(px => px.Cpf.Equals(cpf.Value));
    }
}