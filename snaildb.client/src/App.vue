<script setup lang="ts">
    import { RouterView } from 'vue-router'
    import Nav_Component from '@/components/Nav_Component.vue'
</script>

<template>
    <Nav_Component />
    <RouterView />
</template>

<style>
    body {
        background-color: #16181c;
        color: #fff;
    }
</style>
