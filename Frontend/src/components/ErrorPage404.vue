<template>
    <div class="notfound">
        <h1>404 - Site not found</h1>
        The route <code>{{ $router.history.current.path }}</code> was not found!
        <a href="#" @click="$router.go(-1)">Go back</a>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';

    @Component({
        components: {
        }
    })
    export default class ErrorPage404 extends Vue {
        constructor() {
            super();
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
