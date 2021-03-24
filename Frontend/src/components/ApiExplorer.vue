<template>
    <div class="apiExplorer">
        <iframe id="apiFrame" src="https://localhost:44313/swagger/index.html"></iframe>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';

    @Component
    export default class Home extends Vue {
        @Prop() private msg!: string;
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    #apiFrame {
        min-width: 100%;
        height: 85vh;
    }
</style>
