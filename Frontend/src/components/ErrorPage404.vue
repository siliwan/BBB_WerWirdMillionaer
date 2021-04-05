<template>
    <b-container class="sitenotfound">
        <b-row>
            <b-col>
                <h1>404 - Site not found</h1>
            </b-col>
        </b-row>
        <b-row>
            <b-col>
                <p>The route <code>{{ $router.history.current.path }}</code> was not found!</p>
            </b-col>
        </b-row>
        <b-row>
            <b-col>
                <b-button-group>
                    <b-button @click="$router.go(-1)" variant="primary">Go back</b-button>
                    <b-button @click="$router.push('/')" variant="primary">Home</b-button>
                </b-button-group>
            </b-col>
        </b-row>
    </b-container>
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
