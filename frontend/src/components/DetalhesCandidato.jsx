import { useEffect, useState } from 'react'
import { obterCandidato } from '../services/api'

export default function DetalhesCandidato({ id, onVoltar }) {
  const [candidato, setCandidato] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  useEffect(() => {
    const controller = new AbortController()

    async function carregar() {
      setCarregando(true)
      setCandidato(null)
      setErro('')

      try {
        const dados = await obterCandidato(id, controller.signal)

        if (!controller.signal.aborted) {
          setCandidato(dados)
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
  }, [id])

  return (
    <section className="painel" aria-busy={carregando}>
      <button type="button" onClick={onVoltar}>
        Voltar para candidatos
      </button>

      <h2>Detalhes do candidato</h2>

      {carregando && <p role="status">Carregando detalhes...</p>}
      {erro && <p className="erro" role="alert">{erro}</p>}

      {!carregando && !erro && candidato && (
        <dl className="detalhes">
          <dt>Nome completo</dt>
          <dd>{candidato.nomeCompleto}</dd>

          <dt>E-mail</dt>
          <dd>{candidato.email}</dd>

          <dt>Telefone</dt>
          <dd>{candidato.telefone || 'Não informado'}</dd>

          <dt>Área ou cargo de interesse</dt>
          <dd>{candidato.areaInteresse || 'Não informado'}</dd>

          <dt>Resumo profissional</dt>
          <dd className="resumo">
            {candidato.resumoProfissional || 'Não informado'}
          </dd>
        </dl>
      )}
    </section>
  )
}