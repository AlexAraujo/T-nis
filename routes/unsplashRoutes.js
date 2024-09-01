const express = require('express');
const { searchImages } = require('../controllers/unsplashController');
const router = express.Router();

router.post('/search', searchImages);

module.exports = router;