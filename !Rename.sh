#!/bin/bash
# Usage: Run from repo root. Edits all files and renames files/folders from $SOURCE to $TARGET


# Set the source and target strings (case-sensitive base)
SOURCE="ContainerPipelineTest"
TARGET="ContainerTestImage"

# Function for case-preserving replacement using Perl
replace_case_preserve() {
    local file="$1"
    local src="$2"
    local tgt="$3"
    perl -i -pe '
        sub preserve_case {
            my ($from, $to, $str) = @_;
            $str =~ s{($from)}{
                my $m = $1;
                my $rep = $to;
                if ($m eq lc($m)) {
                    $rep = lc($to);
                } elsif ($m eq uc($m)) {
                    $rep = uc($to);
                } elsif (substr($m,0,1) eq uc(substr($m,0,1))) {
                    $rep = ucfirst(lc($to));
                }
                $rep;
            }gei;
            return $str;
        }
        $_ = preserve_case("$src", "$tgt", $_);
    ' "$file"
}


# Replace content in all files (excluding binary files), preserving case
find . -type f ! -path '*/.git/*' -exec grep -Iq . {} \; -and -print | while read -r file; do
    replace_case_preserve "$file" "$SOURCE" "$TARGET"
done


# Rename files and directories (case-sensitive, not case-preserving)
find . -depth -name "*${SOURCE}*" | while read -r path; do
    newpath="$(dirname "$path")/$(basename "$path" | perl -pe 's/${SOURCE}/${TARGET}/g')"
    mv "$path" "$newpath"
done
