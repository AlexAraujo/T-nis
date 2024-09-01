const axios = require('axios');
const { Configuration, OpenAIApi } = require('openai');

const configuration = new Configuration({
  apiKey: process.env.OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);

const resumirDescricao = async (descricao) => {
  try {
    const response = await openai.createCompletion({
      model: "text-davinci-003",
      prompt: `Resuma isso em 4 palavras: "${descricao}"`,
      max_tokens: 10,
    });
    return response.data.choices[0].text.trim();
  } catch (error) {
    console.error('Erro ao resumir descrição:', error);
    return descricao; // Retorna a descrição original em caso de erro
  }
};

const searchImages = async (req, res) => {
  const { descricao, cor, estilo } = req.body;
  try {
    const response = await axios.get('https://api.unsplash.com/search/photos', {
      params: { query: descricao, color: cor, orientation: estilo },
      headers: {
        Authorization: `Client-ID ${process.env.UNSPLASH_ACCESS_KEY}`,
      },
    });

    const images = await Promise.all(response.data.results.map(async (img) => {
      const resumoDescricao = await resumirDescricao(img.description || img.alt_description || '');
      return {
        ...img,
        resumoDescricao,
      };
    }));

    res.json({ images });
  } catch (error) {
    console.error('Erro ao buscar imagens:', error);
    res.status(500).json({ error: 'Erro ao buscar imagens' });
  }
};

module.exports = { searchImages };