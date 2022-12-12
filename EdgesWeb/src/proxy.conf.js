const PROXY_CONFIG = [
  {
    context: [
      "/Edges"
    ],
    target: "https://localhost:7049/",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
