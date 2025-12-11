module.exports = {
  content: [
    "./{src,pages,components,app}/**/*.{ts,tsx,js,jsx,html}",
    "./src/**/*.{ts, tsx, js, jsx}",
    "!./{src,pages,components,app}/**/*.{stories,spec}.{ts,tsx,js,jsx,html}",
    //     ...createGlobPatternsForDependencies(__dirname)
  ],
  theme: {
    extend: {
      fontFamily: {
        Roboto: ["var(--font-roboto)"],
        Poppins: ["var(--font-poppins)"],
      }
    },
  },
  plugins: [],
};
