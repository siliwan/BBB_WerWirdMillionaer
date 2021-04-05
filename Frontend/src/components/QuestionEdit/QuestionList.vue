<template>
    <div class="questionList container">
        <b-alert :show="hasWarning" variant="danger">{{warningText}}</b-alert>
        <b-table 
            id="questionListTable"
            striped 
            hover 
            outlined
            responsive
            :items="questions" 
            isBusy="isLoading" 
            :fields="fields"
            :select-mode="'single'"
            selectable
            :per-page="perPage"
            :current-page="currentPage"
            @row-selected="onRowSelected" />
        <b-pagination
            v-model="currentPage"
            :total-rows="questions.length"
            :per-page="perPage"
            aria-controls="questionListTable" />
        <div class="border p-2 mb-3" v-if="isSelected">
            <p>
                Selected Question with id {{ selected.id }}:
                <br>
                <small>{{ selected.questionText }}</small>
            </p>
            
        </div>
        <b-button-group>
            <b-button variant="success" to="/questions/new">New</b-button>
            <b-button :disabled="!isSelected" variant="primary" :to="`/questions/edit/${selected.id}`">Edit</b-button>
            <b-button :disabled="!isSelected" variant="danger" @click="onDelete">Delete</b-button>
        </b-button-group>
    </div>
</template>
<script lang="ts">
    import QuestionCrud from "@/components/CRUD/QuestionCrud";
    import { HighScore, nameof, Question, QuestionStatistic } from "@/ResponseTypes";
    import AuthApi from "@/services/AuthApi";
    //@ts-ignore
    import { NavigationGuardNext, Route } from "node_modules/vue-router/types";
    import { Component, Vue } from 'vue-property-decorator';

    @Component({
        components: {
            
        }
    })
    export default class QuestionList extends Vue {

        private warningText: string = '';
        private questions: Question[] = [];
        private isLoading: boolean = true;

        private currentPage: number = 0;
        private perPage: number = 10;

        private fields = [
            { key: nameof<Question>("id"), label: nameof<Question>("id") },
            { key: nameof<Question>("questionText"), label: "Text" },
            { key: nameof<QuestionStatistic>("answeredCorrect"), label: "Correct", formatter: this.questionStatisticFormatter },
            { key: nameof<QuestionStatistic>("answeredWrong"), label: "Wrong", formatter: this.questionStatisticFormatter },
        ]
        private selected: Question = new Question();

        constructor() {
            super();
        }

        questionStatisticFormatter(currentValue: string, fieldKey: any, item: Question) {
            switch (fieldKey) {
                case nameof<QuestionStatistic>('answeredCorrect'):
                    return item.questionStatistic?.answeredCorrect;
            
                case nameof<QuestionStatistic>('answeredWrong'):
                    return item.questionStatistic?.answeredWrong;
            }

            return currentValue;
        }

        async mounted() {
            await this.reload();
        }

        async reload() {
            this.isLoading = true;
            this.questions = await QuestionCrud.getAll();
            this.isLoading = false;
        }

        async onDelete() {
            const choice = await this.$bvModal.msgBoxConfirm("Do you really want to delete this question?", {
                headerBgVariant: 'danger',
                okTitle: 'Delete',
                okVariant: 'danger',

            }) ?? false;

            if(choice) {
                try {
                    await QuestionCrud.delete(this.selected.id);
                    this.selected = new Question();
                    await this.reload()
                } catch (error) {
                    this.warningText = "Could not delete the entry.";
                }
            }
        }

        get hasWarning() {
            return this.warningText !== undefined && this.warningText.trim() !== '';
        }

        get isSelected() {
            return this.selected !== undefined && this.selected.id !== undefined;
        }

        onRowSelected(item: Question): void {
            if(Array.isArray(item)) {
                item = item[0];
            }

            this.selected = item;
        }

        onCategoriesFormat(currentValue: string, fieldKey: any, item: HighScore) {
            return item.categoriesSeperated ?? item.categories.split(',').join(', ');
        }

        public static async beforeEnter(to: Route, 
                                        from: Route, 
                                        next: NavigationGuardNext<Vue>) {
            let isAuthenticated = false; 
            try {
               isAuthenticated = await AuthApi.IsLoggedIn();
            } catch (error) { }
            if(!isAuthenticated) next('/')
            else next()
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
