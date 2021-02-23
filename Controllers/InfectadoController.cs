using System;
using Api.Data.Collections;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class InfectadoController : ControllerBase
    {
        Data.MongoDB _mongoDB;
        IMongoCollection<Infectado> _infectadosCollection;

        public InfectadoController(Data.MongoDB mongoDB)
        {
            _mongoDB = mongoDB;
            _infectadosCollection = _mongoDB.DB.GetCollection<Infectado>(typeof(Infectado).Name.ToLower());
        }

        [HttpPost]
        public ActionResult SalvarInfectado([FromBody] InfectadoDto dto)
        {
            var infectado = new Infectado(dto.Nome, dto.DataNascimento, dto.Sexo, dto.Latitude, dto.Longitude, dto.Curado);

            _infectadosCollection.InsertOne(infectado);

            return StatusCode(201, "Infectado adicionado com sucesso");
        }

        [HttpGet]
        public ActionResult ObterInfectados()
        {
            var infectados = _infectadosCollection.Find(Builders<Infectado>.Filter.Empty).ToList();

            return Ok(infectados);
        }

        [HttpPatch]
        public ActionResult AtualizarInfectados([FromBody] InfectadoDto dto)
        {
            _infectadosCollection.UpdateOne(Builders<Infectado>.Filter.Where(_ => _.Nome == dto.Nome), Builders<Infectado>.Update.Set("curado", dto.Curado));

            return Ok("Atualizado com sucesso");
        }

        [HttpPut]
        public ActionResult CorrigirInfectados([FromBody] InfectadoDto dto)
        {
            var filter = Builders<Infectado>.Filter.Eq("nome", dto.Nome);
            var update = Builders<Infectado>.Update.Set("dataNascimento", dto.DataNascimento)
                                                    .Set("sexo", dto.Sexo)
                                                    .Set("curado", dto.Curado);
            _infectadosCollection.UpdateOne(filter, update);

            return Ok("Atualizado com sucesso");
        }

        [HttpDelete("{nome}")]
        public ActionResult Delete(string nome)
        {
            _infectadosCollection.DeleteOne(Builders<Infectado>.Filter.Eq("nome", nome));

            return Ok("Apagado com sucesso");
        }
    }
}