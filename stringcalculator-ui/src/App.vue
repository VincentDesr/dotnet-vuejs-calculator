<template>
  <v-app>
    <v-app-bar color="primary" elevation="2">
      <v-app-bar-title>
        <v-icon start>mdi-calculator-variant</v-icon>
        String Calculator
      </v-app-bar-title>
    </v-app-bar>

    <v-main>
      <Calculator />
    </v-main>

    <v-footer app class="text-center">
      <v-col>
        <span class="text-caption">
          Built with Vue 3, TypeScript, Vite, and Vuetify 3
        </span>
      </v-col>
    </v-footer>
  </v-app>
</template>

<script setup lang="ts">
import Calculator from './components/Calculator.vue';
</script>

<style>
html, body {
  margin: 0;
  padding: 0;
  height: 100%;
}
</style>
