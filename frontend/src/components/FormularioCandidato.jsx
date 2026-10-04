import { useState } from 'react'
import {cadastrarCandidato, importarCurriculo,} from '../services/api'

const camposVazios = {
  nomeCompleto: '',
  email: '',
  telefone: '',
  areaInteresse: '',
  resumoProfissional: '',
}

export default function FormularioCandidato({ onSalvo }) {
  const [campos, setCampos] = useState({ ...camposVazios })
  const [ocupado, setOcupado] = useState(false)
  const [mensagem, setMensagem] = useState('')
  const [erro, setErro] = useState('')

  function alterar(evento) {
    const { name, value } = evento.target

    setCampos(atuais => ({
      ...atuais,
      [name]: value,
    }))
  }

  async function importar(evento) {
    const arquivo = evento.target.files?.[0]

    // Permite selecionar novamente o mesmo arquivo.
    evento.target.value = ''

    if (!arquivo) return

    setMensagem('')
    setErro('')

    if (!arquivo.name.toLowerCase().endsWith('.pdf')) {
      setErro('Selecione um arquivo PDF. Você pode cadastrar manualmente.')
      return
    }

    if (arquivo.size > 5 * 1024 * 1024) {
      setErro('O PDF deve ter até 5 MB. Você pode cadastrar manualmente.')
      return
    }

    if (arquivo.size === 0) {
      setErro('O arquivo está vazio. Você pode cadastrar manualmente.')
      return
    }

    setOcupado(true)

    try {
      const dados = await importarCurriculo(arquivo)

      // Preserva os campos que a pessoa já preencheu.
      setCampos(atuais => ({
        ...atuais,
        nomeCompleto: atuais.nomeCompleto.trim()
          ? atuais.nomeCompleto
          : dados.nomeCompleto || '',
        email: atuais.email.trim()
          ? atuais.email
          : dados.email || '',
        telefone: atuais.telefone.trim()
          ? atuais.telefone
          : dados.telefone || '',
      }))

      setMensagem(
        'PDF lido. Revise as sugestões e complete os campos. ' +
        'Os dados já preenchidos foram mantidos.'
      )
    } catch (error) {
      setErro(
        `${error.message} O cadastro manual continua disponível.`
      )
    } finally {
      setOcupado(false)
    }
  }

  async function salvar(evento) {
    evento.preventDefault()
    setMensagem('')
    setErro('')

    const dados = Object.fromEntries(
      Object.entries(campos).map(([chave, valor]) => [
        chave,
        valor.trim(),
      ])
    )

    if (!dados.nomeCompleto || !dados.email) {
      setErro('Informe o nome completo e o e-mail.')
      return
    }

    setOcupado(true)

    try {
      await cadastrarCandidato(dados)

      setCampos({ ...camposVazios })
      setMensagem('Cadastro salvo com sucesso.')
      onSalvo()
    } catch (error) {
      setErro(error.message)
    } finally {
      setOcupado(false)
    }
  }

  return (
    <section className="painel formulario">
      <h2>Novo candidato</h2>
      <p>
        Preencha os dados ou importe um PDF para ajudar no preenchimento.
        O arquivo é opcional.
      </p>

      <form onSubmit={salvar}>
        <fieldset disabled={ocupado}>
          <label htmlFor="curriculo">Importar currículo — PDF até 5 MB</label>
          <input
            id="curriculo"
            type="file"
            accept=".pdf,application/pdf"
            onChange={importar}
          />

          <label htmlFor="nomeCompleto">Nome completo *</label>
          <input
            id="nomeCompleto"
            name="nomeCompleto"
            value={campos.nomeCompleto}
            onChange={alterar}
            maxLength={200}
            autoComplete="name"
            required
          />

          <label htmlFor="email">E-mail *</label>
          <input
            id="email"
            name="email"
            type="email"
            value={campos.email}
            onChange={alterar}
            maxLength={254}
            autoComplete="email"
            required
          />

          <label htmlFor="telefone">Telefone</label>
          <input
            id="telefone"
            name="telefone"
            type="tel"
            value={campos.telefone}
            onChange={alterar}
            maxLength={30}
            autoComplete="tel"
          />

          <label htmlFor="areaInteresse">Área ou cargo de interesse</label>
          <input
            id="areaInteresse"
            name="areaInteresse"
            value={campos.areaInteresse}
            onChange={alterar}
            maxLength={150}
          />

          <label htmlFor="resumoProfissional">Resumo profissional</label>
          <textarea
            id="resumoProfissional"
            name="resumoProfissional"
            value={campos.resumoProfissional}
            onChange={alterar}
            maxLength={4000}
            rows={5}
          />

          <button type="submit">Salvar candidato</button>
        </fieldset>

        {ocupado && <p role="status">Processando...</p>}
        {mensagem && (
          <p className="sucesso" role="status">{mensagem}</p>
        )}
        {erro && <p className="erro" role="alert">{erro}</p>}
      </form>
    </section>
  )
}