<template>
    <div class="quizstart">
        <h1>Quiz</h1>
        <b-alert :show="warningText !== '' && warningText !== undefined" variant="danger" dismissible @dismissed="onDismiss()">{{warningText}}</b-alert>
        <b-form-select v-model="selected" :options="options" multiple></b-form-select>
        <b-btn @click="startGame()" variant="primary">Start Game</b-btn>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import { NavigationGuardNext, Route } from 'vue-router';
    import GameApi from "@/services/GameApi";
    import { Category, PlayState } from '@/ResponseTypes';
    import CategoryCrud from "@/components/CRUD/CategoryCrud";

    @Component({
        components: {
            
        }
    })
    export default class QuizStart extends Vue {

        private selected: Category[] = [];
        private options: { value: Category, text: string }[] = []
        private warningText: string = '';

        constructor() {
            super();
        }

        async mounted() {
            let categories = await CategoryCrud.getAll();

            if(categories == null) return;

            this.options = categories.map(cat => {return { value: cat, text: cat.name }});
        }

        public onDismiss() {
            this.warningText = '';
        }

        public static async beforeEnter(to: Route, 
                                  from: Route, 
                                  next: NavigationGuardNext<Vue>) {
            let state = await GameApi.CurrentState();

            if(state == PlayState.Playing) next('quiz/play')
            else next()

            
        }

        public async startGame() {

            if(this.selected.length === 0) {
                this.warningText = "You need to select at least 1 category!";
                return;
            }

            await GameApi.StartGame(this.selected);
            this.$router.push('play')
        }

    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
