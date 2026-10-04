import { useEffect, useState } from 'react'
import { listarCandidatos } from './services/api'
import './App.css'
import FormularioCandidato from './components/FormularioCandidato'
import DetalhesCandidato from './components/DetalhesCandidato'

function App() {
  const [candidatos, setCandidatos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [atualizacao, setAtualizacao] = useState(0)
  const [candidatoSelecionado, setCandidatoSelecionado] = useState(null)

  useEffect(() => {
    const controller = new AbortController()

    async function carregar() {
      setCarregando(true)
      setErro('')

      try {
        const dados = await listarCandidatos(controller.signal)

        if (!controller.signal.aborted) {
          setCandidatos(dados)
        }
      } catch (error) {
        if (!controller.signal.aborted) {
          setErro(error.message)
        }
      } finally {
        if (!controller.signal.aborted) {
          setCarregando(false)
        }
      }
    }

    carregar()

    return () => controller.abort()
  }, [atualizacao])

  if (candidatoSelecionado !== null) {
  return (
    <main className="container">
      <header className="cabecalho">
        <div>
          <h1>CVHub</h1>
          <p>Cadastro e consulta de candidatos</p>
        </div>
      </header>

      <DetalhesCandidato
        id={candidatoSelecionado}
        onVoltar={() => setCandidatoSelecionado(null)}
      />
    </main>
  )
}

  return (
    <main className="container">
      <header className="cabecalho">
        <div>
          <h1>CVHub</h1>
          <p>Cadastro e consulta de candidatos</p>
        </div>

        <button
          type="button"
          disabled={carregando}
          onClick={() => setAtualizacao(valor => valor + 1)}
        >
          Atualizar
        </button>
      </header>
      <FormularioCandidato onSalvo={() => setAtualizacao(valor => valor + 1)}
/>
      <section className="painel" aria-busy={carregando}>
        <h2>Candidatos</h2>

        {carregando && <p role="status">Carregando candidatos...</p>}

        {erro && <p className="erro" role="alert">{erro}</p>}

        {!carregando && !erro && candidatos.length === 0 && (
          <p>Nenhum candidato cadastrado.</p>
        )}

        {!carregando && !erro && candidatos.length > 0 && (
          <div className="tabela-container">
            <table>
              <thead>
                <tr>
                  <th scope="col">Nome</th>
                  <th scope="col">E-mail</th>
                  <th scope="col">Telefone</th>
                  <th scope="col">Área de interesse</th>
                  <th scope="col">Ações</th>
                </tr>
              </thead>

              <tbody>
                {candidatos.map(candidato => (
                  <tr key={candidato.id}>
                    <td>{candidato.nomeCompleto}</td>
                    <td>{candidato.email}</td>
                    <td>{candidato.telefone || 'Não informado'}</td>
                    <td>{candidato.areaInteresse || 'Não informada'}</td>
                    <td> <button type="button" onClick={() => setCandidatoSelecionado(candidato.id)} aria-label={`Ver detalhes de ${candidato.nomeCompleto}`}>
                      Ver detalhes
                    </button>
                  </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </main>
  )
}

export default App