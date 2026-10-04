const API_URL = import.meta.env.VITE_API_URL

export async function listarCandidatos(signal) {
  if (!API_URL) {
    throw new Error('O endereço da API não foi configurado.')
  }

  let resposta

  try {
    resposta = await fetch(`${API_URL}/api/candidatos`, { signal })
  } catch (erro) {
    if (erro.name === 'AbortError') throw erro

    throw new Error(
      'Não foi possível conectar à API. Confira se o backend está em execução.'
    )
  }

  if (!resposta.ok) {
    const dados = await resposta.json().catch(() => null)

    throw new Error(
      dados?.mensagem || 'Não foi possível carregar os candidatos.'
    )
  }

  return resposta.json()
}

async function enviar(caminho, opcoes) {
  if (!API_URL) {
    throw new Error('O endereço da API não foi configurado.')
  }

  let resposta

  try {
    resposta = await fetch(`${API_URL}${caminho}`, opcoes)
  } catch {
    throw new Error(
      'Não foi possível conectar à API. Tente novamente.'
    )
  }

  const dados = await resposta.json().catch(() => null)

  if (!resposta.ok) {
    const erros = dados?.erros || dados?.errors
    const detalhes = erros
      ? Object.values(erros).flat().join(' ')
      : ''

    throw new Error(
      [dados?.mensagem, detalhes].filter(Boolean).join(' ') ||
      (resposta.status === 413
        ? 'O arquivo enviado é muito grande.'
        : 'Não foi possível concluir a operação.')
    )
  }

  return dados
}

export function cadastrarCandidato(candidato) {
  return enviar('/api/candidatos', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(candidato),
  })
}

export function importarCurriculo(arquivo) {
  const formulario = new FormData()
  formulario.append('arquivo', arquivo)

  return enviar('/api/curriculos/extrair', {
    method: 'POST',
    body: formulario,
  })
}

export async function obterCandidato(id, signal) {
  if (!API_URL) {
    throw new Error('O endereço da API não foi configurado.')
  }

  let resposta

  try {
    resposta = await fetch(`${API_URL}/api/candidatos/${id}`, {
      signal,
    })
  } catch (erro) {
    if (erro.name === 'AbortError') throw erro

    throw new Error('Não foi possível conectar à API.')
  }

  const dados = await resposta.json().catch(() => null)

  if (!resposta.ok) {
    throw new Error(
      dados?.mensagem || 'Não foi possível consultar o candidato.'
    )
  }

  return dados
}