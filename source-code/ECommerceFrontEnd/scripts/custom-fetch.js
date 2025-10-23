const https = require("https");
const fetch = require("node-fetch");

/**
 * Custom fetch cho môi trường dev (HTTPS self-signed)
 */
async function customFetch(url, init) {
    const agent = new https.Agent({ rejectUnauthorized: false });
    return fetch(url, { ...(init || {}), agent });
}

module.exports = { customFetch };
