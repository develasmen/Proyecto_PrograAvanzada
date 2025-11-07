using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.AccesoADatos.Cliente
{
    public class ClienteRepository
    {
        private readonly Contexto _contexto;

        public ClienteRepository()
        { 
            _contexto = new Contexto();
        }

        public int Guardar(ClienteDto nuevoCliente)
        {
            var clienteDb = new ClienteAd()
            {
                Nombre = nuevoCliente.Nombre,
                CedulaJuridica = nuevoCliente.CedulaJuridica,
                Correo = nuevoCliente.Correo,
                Telefono = nuevoCliente.Telefono,
                Direccion = nuevoCliente.Direccion,
                Estado = true,
                FechaDeRegistro = DateTime.Now
            };

            _contexto.Clientes.Add(clienteDb);
            return _contexto.SaveChanges();
        }

        public int Actualizar(ClienteDto cliente)
        {
            var clienteEnDb = _contexto.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
            if (clienteEnDb != null)
            {
                clienteEnDb.Nombre = cliente.Nombre;
                clienteEnDb.CedulaJuridica = cliente.CedulaJuridica;
                clienteEnDb.Correo = cliente.Correo;
                clienteEnDb.Telefono = cliente.Telefono;
                clienteEnDb.Direccion = cliente.Direccion;
                clienteEnDb.Estado = cliente.Estado;
                clienteEnDb.FechaDeModificacion = DateTime.Now;

                _contexto.Entry(clienteEnDb).State = EntityState.Modified;
                return _contexto.SaveChanges();
            }
            return 0;
        }

        public int Eliminar(int id)
        {
            var clienteEnDb = _contexto.Clientes.FirstOrDefault(c => c.Id == id);
            if (clienteEnDb != null)
            {
                _contexto.Clientes.Remove(clienteEnDb);
                return _contexto.SaveChanges();
            }
            return 0;
        }


        public ClienteDto ObtenerPorId(int id)
        {
            var c = _contexto.Clientes.FirstOrDefault(cliente => cliente.Id == id);
            if (c == null) return null;

            return new ClienteDto()
            {
                Id = c.Id,
                Nombre = c.Nombre,
                CedulaJuridica = c.CedulaJuridica,
                Correo = c.Correo,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                Estado = c.Estado,
                FechaDeRegistro = c.FechaDeRegistro,
                FechaDeModificacion = c.FechaDeModificacion
            };
        }

        public List<ClienteDto> ObtenerTodos()
        {
            return _contexto.Clientes.Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                CedulaJuridica = c.CedulaJuridica,
                Correo = c.Correo,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                Estado = c.Estado
            }).ToList();
        }

        public List<ClienteDto> BuscarPorNombreOCedula(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
            {
                return ObtenerTodos();
            }

            return _contexto.Clientes
                .Where(c => c.Nombre.Contains(criterio) || c.CedulaJuridica.Contains(criterio))
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    CedulaJuridica = c.CedulaJuridica,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Estado = c.Estado
                }).ToList();
        }
    }
}
