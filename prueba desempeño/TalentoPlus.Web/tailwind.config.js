/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Views/**/*.html",
    "./wwwroot/**/*.html"
  ],
  theme: {
    extend: {
      colors: {
        primary: '#0f172a',
        secondary: '#475569',
        accent: '#0ea5e9',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
