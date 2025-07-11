# Projeto em desenvolvimento
As informações presentes neste projeto podem sofrer alterações ao longo do semestre.

## Projeto avaliativo - Disciplina da UNIFTEC
Projeto desenvolvido para a disciplina de Projeto de Sistemas para Web no 1° semestre de 2025 da UNIFTEC Caxias do Sul.

### Colaboradores
- [Bruno Lopes Madeira](https://github.com/brunolmadeira)
- [Julio Felipe Prá Spigolan](https://github.com/JulioFelipePS)

## Recursos disponíveis
- Autenticação via token JWT
- Recuperação de senha com token de 6 dígitos (válido por 15 minutos)
- Cadastro de usuário com validação de e-mail e username
- Verificação de disponibilidade de e-mail e username
- Atualização de perfil (nome, celular, pronome, bio, link, data de nascimento e foto)
- Adicionar/remover seguidor (por ID ou username)
- Excluir conta
- Pesquisar usuários por:
  - ID
  - Username
  - Termo (nome ou username)
- Seguir/Deixar de seguir usuários

## Informações do usuário
### Dados públicos (retornados em pesquisas):
- ID
- Nome completo
- Username
- Foto (Base64)
- Bio (descrição)
- Pronome (gênero)
- Link (perfil)

### Dados retornados no login:
- Token JWT
- ID do usuário
- Username
- E-mail

## Segurança
- Todas as requisições exigem:
  - Header `Authorization` com chave secreta `Bearer ProjetoWebRedeSocial` (para endpoints públicos)
  - Token JWT válido (para endpoints autenticados)
- Senhas armazenadas com hash BCrypt
- Tokens de recuperação de senha:
  - São numéricos de 6 dígitos
  - Válidos por 15 minutos
  - Invalidados após uso
- Dados sensíveis nunca são retornados em pesquisas

> **Nota:** O JSON completo com os exemplos de requisições/respostas está disponível no repositório:  
> [API Usuários - Postman Collection](https://github.com/brunolmadeira/ftec-projetos-web/blob/main/API%20Usu%C3%A1rios.postman_collection.json)
